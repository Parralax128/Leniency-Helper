using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;
using VivHelper.Entities;

namespace Celeste.Mod.LeniencyHelper.Controllers;

[Tracked(true)]
[CustomEntity("LeniencyHelper/Controllers/ConsistentCoreboostDirectionController")]
public class ConsistentCoreboostDirectionController : GenericController
{
    #region Hooks

    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.BounceBlock.Update += ModifyDirection;
    }

    private static ILHook vivHelperUpdateHook;
    public static void LoadVivHelperHooks()
    {
        vivHelperUpdateHook = new ILHook(typeof(ReskinnableBounceBlock).GetMethod("Update"), ModifyDirection);
    }

    private static void ModifyDirection(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        while (cursor.TryGotoNextBestFit(MoveType.After,
            instr => instr.MatchCall(typeof(Calc).GetMethod("SafeNormalize", new System.Type[] { typeof(Vector2) })),
            instr => instr.MatchStfld<BounceBlock>("bounceDir"))

            || (Module.LeniencyHelperModule.ModLoaded("VivHelper") && cursor.TryGotoNextBestFit(MoveType.After,
            instr => instr.MatchCall(typeof(Calc).GetMethod("SafeNormalize", new System.Type[] { typeof(Vector2) })),
            instr => MatchVivHelperBounceDir(instr))))
        {
            cursor.Index--;
            cursor.EmitLdarg0();
            cursor.EmitDelegate(GetBounceDir);
        }

        Log(il);
    }
    private static bool MatchVivHelperBounceDir(Mono.Cecil.Cil.Instruction instr)
    {
        return instr.MatchStfld<ReskinnableBounceBlock>("bounceDir");
    }
    private static Vector2 GetBounceDir(Vector2 origDir, Solid block)
    {
        if (!ControllerInRoom(block)) return origDir;


        Player playerClimbing = block.GetPlayerClimbing();

        if(LeniencyHelperModule.Session.cornerCoreboostDir)
        {
            if (playerClimbing != null)
            {
                return (new Vector2(playerClimbing.Facing == Facings.Right ? block.Left : block.Right,
                    CrossModSupport.GravityHelperImports.currentGravity == 1 ? block.Top : block.Bottom) - block.Center).SafeNormalize();
            }

            else
            {
                Player player = block.Scene.Tracker.GetEntity<Player>();
                if (player == null) return origDir;

                return (new Vector2((block.Right - player.Center.X > player.Center.X - block.Left) ? block.Left : block.Right,
                   CrossModSupport.GravityHelperImports.currentGravity == 1 ? block.Top : block.Bottom) - block.Center).SafeNormalize();
            }
        }
        else
        {
            if (playerClimbing != null)
            {
                return Vector2.UnitX * -(int)playerClimbing.Facing;
            }

            else return Vector2.UnitY * -CrossModSupport.GravityHelperImports.currentGravity;
        }
    }
    private static bool ControllerInRoom(Entity e)
    {
        return e.Scene.Tracker.GetEntity<ConsistentCoreboostDirectionController>() != null;
    }

    #endregion

    private static void Log(object o) => Module.LeniencyHelperModule.Log(o);

    private bool thisAimCorner;
    public ConsistentCoreboostDirectionController(EntityData data, Vector2 offset) : base(data, offset, false)
    {
        thisAimCorner = data.Bool("AimCorner", false);
    }

    public override void GetOldSettings() { }

    public override void Apply(bool fromFlag)
    {
        LeniencyHelperModule.Session.cornerCoreboostDir = thisAimCorner;
    }
    public override void Undo(bool fromFlag)
    {
        
    }
}