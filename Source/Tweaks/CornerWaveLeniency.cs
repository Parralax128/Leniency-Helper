using Monocle;
using Microsoft.Xna.Framework;
using System;
using static Celeste.Mod.LeniencyHelper.CrossModSupport.GravityHelperImports;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using static Celeste.Mod.Helpers.ILCursorExtensions;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CornerWaveLeniency : AbstractTweak<CornerWaveLeniency>
{
    private static ILHook origUpdateHook;
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.WallJump += StartGroundCheck;
        IL.Celeste.Player.OnCollideV += RemoveDiagCCorection;
        origUpdateHook = new ILHook(typeof(Player).GetMethod(nameof(Player.orig_Update)), HookedUpdate);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.WallJump -= StartGroundCheck;
        IL.Celeste.Player.OnCollideV -= RemoveDiagCCorection;
        origUpdateHook.Dispose();
    }

    private static bool groundChecking = false, groundDetected = false, wasDashing = false;
    private static Vector2 origPos;
    private static void HookedUpdate(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        ILLabel refillCheckLabel = il.DefineLabel();
        ILLabel skipRefillLabel = il.DefineLabel();
        ILLabel varJumpTimerLabel = il.DefineLabel();
        ILLabel returnLabel = il.DefineLabel();

        cursor.EmitDelegate(ClearGroundChecking);

        if(cursor.TryGotoNextBestFit(MoveType.Before, 
            i => i.MatchCallvirt<Player>("get_Inventory"),
            i => i.MatchLdfld<PlayerInventory>("NoRefills"),
            i => i.MatchBrtrue(out ILLabel label)))
        {
            cursor.Index--;
            cursor.MarkLabel(refillCheckLabel);

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("onGround")))
            {
                cursor.EmitLdarg0();
                cursor.EmitDelegate((bool orig, Player player) => orig || (player.OnGround() && groundChecking));
            }

            if(cursor.TryGotoNext(i => i.MatchLdfld<Assists>("Invincible"), i => i.MatchBrfalse(out varJumpTimerLabel)))
            {
                cursor.GotoNext(MoveType.After, i => i.MatchCallvirt<Player>("RefillDash"), i => i.MatchPop());
                cursor.MarkLabel(skipRefillLabel);
                
                cursor.GotoPrev(MoveType.Before, i => i.MatchCallvirt<Player>("RefillDash"));
                cursor.EmitDelegate(OnGroundDetected);
                cursor.EmitLdarg0();
                cursor.EmitDelegate(GroundAndCDChecking);
                cursor.EmitBrtrue(skipRefillLabel);

                cursor.EmitLdarg0();
                

                cursor.GotoLabel(varJumpTimerLabel);
                cursor.GotoNext(MoveType.After, i => i.MatchLdarg0());
                cursor.EmitDelegate(ReturnPos);
                cursor.EmitDelegate(GroundChecking);
                cursor.EmitBrtrue(returnLabel);
                
                cursor.EmitLdarg0();
                
                
                cursor.GotoNext(MoveType.After, i => i.MatchCall<Actor>("Update"));
                cursor.EmitDelegate(GroundChecking);
                cursor.EmitBrtrue(refillCheckLabel);
                cursor.MarkLabel(returnLabel);
                cursor.EmitDelegate(ClearGroundChecking);
            }
        }
    }
    private static void OnGroundDetected(Player player)
    {
        if (!groundChecking || !Enabled) return;

        groundDetected = true;

        if (player.DashDir.X != 0f && player.DashDir.Y > 0f && player.Speed.Y > 0f) //performing ultraboost
        {
            player.DashDir.X = Math.Sign(player.DashDir.X);
            player.DashDir.Y = 0f;
            player.Speed.Y = 0f;
            player.Speed.X *= 1.2f;
            player.Ducking = wasDashing || Input.MoveY.Value == currentGravity;
        }

        if (wasDashing) player.SuperJump();
        else player.Jump();
    }
    private static bool GroundChecking() => groundChecking;
    private static bool GroundAndCDChecking(Player player) => player.dashRefillCooldownTimer > 0f && groundChecking;
    private static void ClearGroundChecking() => groundChecking = false;
    private static void ReturnPos(Player player)
    {
        if (!groundChecking) return;

        if (!groundDetected)
        {
            player.Position = origPos;
            player.WallJump(Math.Sign(player.Speed.X)); // if no ground detected - walljumping
        }
        else player.Position.X = origPos.X;
    }

    private static bool CheckDiag(Player player)
    {
        if (!Enabled) return false;

        return player.DashDir.X != 0f && player.DashDir.Y > 0f && player.Speed.Y > 0f;
    }
    private static void RemoveDiagCCorection(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        if (cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdcI4(1),
            instr => instr.MatchCall<Actor>("OnGround")))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(CheckDiag);
            cursor.EmitOr();
        }

        if (cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdcI4(1),
            instr => instr.MatchCall<Actor>("OnGround")))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(CheckDiag);
            cursor.EmitOr();
        }
    }
    private static void StartGroundCheck(On.Celeste.Player.orig_WallJump orig, Player self, int dir)
    {
        if (Enabled && !groundChecking && Math.Sign(self.Speed.X) == dir && CheckCorner(self, -dir))
        {
            groundChecking = true;
            groundDetected = false;
            wasDashing = self.StateMachine.State == 2;
            return;
        }

        orig(self, dir);
    }

    private static bool CheckCorner(Player player, int dir)
    {
        Vector2 checkPos = new Vector2(player.Position.X, currentGravity == 1 ? player.Bottom : player.Top);
        int vertPosCheck = (int)(Math.Abs(player.Speed.Y) * 1.1f * Engine.DeltaTime);

        for (int c=0; c<=vertPosCheck; c++)
        {
            if (LeniencyHelperModule.CollideOnWJdist<Solid>(player, dir, checkPos) 
                && !LeniencyHelperModule.CollideOnWJdist<Solid>(player, dir, checkPos - Vector2.UnitY * currentGravity))
            {
                var s = LeniencyHelperModule.Session;
                for (int x = 0; x < (dir == 1? s.wjDistR : s.wjDistL); x++)
                {
                    if (player.OnGround(checkPos + Vector2.UnitX * dir * x))
                    {
                        origPos = player.Position;
                        player.Position = checkPos + Vector2.UnitX * dir * x;
                        return true;
                    }
                }
            }

            checkPos.Y -= currentGravity;
        }
        return false;
    }
}