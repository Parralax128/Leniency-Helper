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
class ConsistentCoreboostDirectionController : Entity
{
    #region Hooks

    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.BounceBlock.Update += ModifyDirection;
    }

    static ILHook vivHelperUpdateHook;
    public static void LoadVivHelperHooks()
    {
        vivHelperUpdateHook = new ILHook(typeof(ReskinnableBounceBlock).GetMethod("Update"), ModifyDirection);
    }

    static void ModifyDirection(ILContext il)
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
    }
    static bool MatchVivHelperBounceDir(Mono.Cecil.Cil.Instruction instr)
    {
        return instr.MatchStfld<ReskinnableBounceBlock>("bounceDir");
    }
    static Vector2 GetBounceDir(Vector2 origDir, Solid block)
    {
        bool controllerExists, aimCorner;
        CheckControllers(block, out controllerExists, out aimCorner);
        if (!controllerExists) return origDir;

        Player playerClimbing = block.GetPlayerClimbing();

        if (aimCorner)
        {
            if (playerClimbing != null)
            {
                return (new Vector2(playerClimbing.Facing == Facings.Right ? block.Left : block.Right,
                    CrossModSupport.GravityHelperImports.currentGravity == 1 ? block.Top : block.Bottom) - block.Center).SafeNormalize();
            }

            Player player = block.Scene.Tracker.GetEntity<Player>();
            if (player == null) return origDir;

            return (new Vector2((block.Right - player.Center.X > player.Center.X - block.Left) ? block.Left : block.Right,
                CrossModSupport.GravityHelperImports.currentGravity == 1 ? block.Top : block.Bottom) - block.Center).SafeNormalize();
        }

        if (playerClimbing != null) return Vector2.UnitX * -(int)playerClimbing.Facing;

        return Vector2.UnitY * -CrossModSupport.GravityHelperImports.currentGravity;
    }
    static void CheckControllers(Entity e, out bool controllerExists, out bool cornerAim)
    {
        cornerAim = controllerExists = false;
        foreach (ConsistentCoreboostDirectionController controller in e.Scene.Tracker.GetEntities<ConsistentCoreboostDirectionController>())
        {
            if (controller.GetFlagActive) continue;
            if (controller.aimCorner) cornerAim = true;
            controllerExists = true;
        }
    }

    #endregion


    bool aimCorner;
    string stopFlag;

    bool GetFlagActive {
        get
        {
            return stopFlag != "" && SceneAs<Level>().Session.GetFlag(stopFlag);
        }
    }
    public ConsistentCoreboostDirectionController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        aimCorner = data.Bool("AimCorner", false);  
        stopFlag = data.String("StopFlag", "");
    }
}