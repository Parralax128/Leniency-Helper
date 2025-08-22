using System;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class SuperOverWalljump : AbstractTweak<SuperOverWalljump>
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.DashUpdate += PreventStupidThing;
        IL.Celeste.Player.RedDashUpdate += PreventStupidThing;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.DashUpdate -= PreventStupidThing;
        IL.Celeste.Player.RedDashUpdate -= PreventStupidThing;
    }

    private static void PreventStupidThing(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        VariableDefinition wjCheckDir = new VariableDefinition(il.Import(typeof(int)));
        il.Body.Variables.Add(wjCheckDir);

        while (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt<Player>("WallJump")))
        {
            if (cursor.TryGotoPrev(MoveType.Before, instr => instr.MatchCallvirt<Player>("WallJumpCheck")))
            {
                cursor.EmitDup();
                cursor.EmitStloc(wjCheckDir);
                cursor.Index++;

                cursor.EmitLdarg0();
                cursor.EmitLdloc(wjCheckDir);
                cursor.EmitDelegate(CanWJ);
                cursor.EmitAnd();
            }
            cursor.GotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("WallJump"));
        }
    }
    private static bool CanWJ(Player player, int dir)
    {
        if (!Enabled || ClimbJumpCheck(player, dir) || Math.Abs(player.DashDir.Y) > 0.2f)
        {
            return true;
        }

        if (player.OnGround() || player.CollideCheck<JumpThru>())
        {
            return false;
        }

        Vector2 checkPos = player.Position;

        for (int c = 0; c < DynamicCornerCorrection.GetDynamicCorrection(
            (int)CrossModSupport.ExtendedVariantImports.CornerCorrection.Y, player, true); c++)
        {
            if (!WallJumpCheckAt(player, dir, checkPos))
            {
                return false;
            }
            checkPos.Y -= CrossModSupport.GravityHelperImports.currentGravity;
        }
        return true;
    }
    private static bool WallJumpCheckAt(Player player, int dir, Vector2 at, bool useOrig = true)
    {
        Vector2 savePos = player.Position;
        player.Position = at;

        bool saveUseOrig = WallCoyoteFrames.useOrigWJCheck;
        if (useOrig) WallCoyoteFrames.useOrigWJCheck = true;
        bool result = player.WallJumpCheck(dir);
        if (useOrig) WallCoyoteFrames.useOrigWJCheck = saveUseOrig;

        player.Position = savePos;

        return result;
    }
    private static bool ClimbJumpCheck(Player player, int dir)
    {
        return ((int)player.Facing == dir && Input.GrabCheck && player.Stamina > 0f && player.Holding == null &&
            !ClimbBlocker.Check(player.Scene, player, player.Position + Vector2.UnitX * 3f * dir));
    }
}