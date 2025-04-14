using Monocle;
using Microsoft.Xna.Framework;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Celeste.Mod.Helpers;
using static Celeste.Mod.Helpers.ILCursorExtensions;
using System.Reflection;
using Celeste.Mod.LeniencyHelper.CrossModSupport;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class RemoveDBlockCCorection
{
    public static bool isEnabled => LeniencyHelperModule.Session.Tweaks["RemoveDBlockCCorection"].Enabled;
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.OnCollideH += CustomOnCollideH;
        IL.Celeste.Player.OnCollideV += CustomOnCollideV;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.OnCollideH -= CustomOnCollideH;
        IL.Celeste.Player.OnCollideV -= CustomOnCollideV;
    }
    public static void CustomOnCollideH(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        ILLabel label = null;

        if(cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchCall<Vector2>("op_Addition"),
            instr => instr.MatchCallvirt<Player>("DuckFreeAt")))
        {
            if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchBrfalse(out ILLabel label)))
            {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(CollidingDBlockX);
                cursor.EmitNot();
                cursor.EmitAnd();
            }
        }

        if (cursor.TryGotoNextBestFit(MoveType.After,
            instr => instr.MatchLdarg0(),
            instr => instr.MatchLdloc3(),
            instr => instr.MatchCallvirt<Player>("DashCorrectCheck"),
            instr => instr.MatchBrtrue(out label)))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(CollidingDBlockX);

            cursor.EmitBrtrue(label);
        }
    }
    private static bool CollidingDBlockX(Player player)
    {
        return player.CollideCheck<DreamBlock>(player.Position + Vector2.UnitX
            * Math.Sign(player.Speed.X)) && isEnabled;
    }
    private static bool CollidingDBlockY(Vector2 at, Player player, int shift)
    {
        return player.CollideCheck<DreamBlock>(at + Vector2.UnitY * shift * GravityHelperImports.currentGravity) && isEnabled;
    }
    public static void CustomOnCollideV(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        ILLabel label = il.DefineLabel();

        if (cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdcI4(1),
            instr => instr.MatchCall<Actor>("OnGround")))
        {
            cursor.EmitLdarg0();
            cursor.EmitLdfld(typeof(Entity).GetField("Position", BindingFlags.Instance | BindingFlags.Public));
            cursor.EmitLdarg0();
            cursor.EmitLdcI4(1);
            cursor.EmitDelegate(CollidingDBlockY);

            cursor.EmitOr();
        }

        if (cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdcI4(1),
            instr => instr.MatchCall<Actor>("OnGround")))
        {
            cursor.EmitLdarg0();
            cursor.EmitLdfld(typeof(Entity).GetField("Position", BindingFlags.Instance | BindingFlags.Public));
            cursor.EmitLdarg0();
            cursor.EmitLdcI4(1);
            cursor.EmitDelegate(CollidingDBlockY);

            cursor.EmitOr();
        }

        if(cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchCall<Vector2>("op_Addition"),
            instr => instr.MatchCall<Entity>("CollideCheck")))
        {
            if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchBrtrue(out ILLabel label)))
            {
                cursor.EmitLdarg0();
                cursor.EmitLdfld(typeof(Entity).GetField("Position", BindingFlags.Instance | BindingFlags.Public));
                cursor.EmitLdarg0();
                cursor.EmitLdcI4(-1);
                cursor.EmitDelegate(CollidingDBlockY);

                cursor.EmitOr();
            }
        }

        if (cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchCall<Vector2>("op_Addition"),
            instr => instr.MatchCall<Entity>("CollideCheck")))
        {
            if (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchBrtrue(out ILLabel label)))
            {
                cursor.EmitLdarg0();
                cursor.EmitLdfld(typeof(Entity).GetField("Position", BindingFlags.Instance | BindingFlags.Public));
                cursor.EmitLdarg0();
                cursor.EmitLdcI4(-1);
                cursor.EmitDelegate(CollidingDBlockY);

                cursor.EmitOr();
            }
        }
    }
}