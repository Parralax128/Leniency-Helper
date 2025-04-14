using Celeste.Mod.LeniencyHelper.CrossModSupport;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Transactions;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class NoFailedTech
{
    private static ILHook dashCoroutineHook;
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Jump += JumpToTech;
        On.Celeste.Player.OnCollideV += ProtectDashAttack;
        On.Celeste.Player.Update += ChangeProtectedDashAttack;
        On.Celeste.Player.DashBegin += CheckDucking;
        dashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), OnCoroutine);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Jump -= JumpToTech;
        On.Celeste.Player.OnCollideV -= ProtectDashAttack;
        On.Celeste.Player.Update -= ChangeProtectedDashAttack;
        On.Celeste.Player.DashBegin -= CheckDucking;
        dashCoroutineHook.Dispose();
    }

    private static void JumpToTech(On.Celeste.Player.orig_Jump orig, Player self, bool particles = true, bool playSfx = true)
    {
        if (LeniencyHelperModule.Session.Tweaks["NoFailedTech"].Enabled && self.CanUnDuck &&
            (self.onGround && self.dashAttackTimer == 0f? LeniencyHelperModule.Session.protectedDashAttackTimer : self.dashAttackTimer) > 0f)
        {
            self.Ducking = LeniencyHelperModule.Session.dashCrouched;
            self.SuperJump();
            return;
        }

        orig(self, particles, playSfx);
    }
    private static void ChangeProtectedDashAttack(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);

        var s = LeniencyHelperModule.Session;

        if (s.protectedDashAttackTimer > 0f)
            s.protectedDashAttackTimer -= Engine.DeltaTime;
    }
    private static void ProtectDashAttack(On.Celeste.Player.orig_OnCollideV orig, Player self, CollisionData data)
    {
        if (LeniencyHelperModule.Session.Tweaks["NoFailedTech"].Enabled)
            LeniencyHelperModule.Session.protectedDashAttackTimer = self.dashAttackTimer;

        orig(self, data);            
    }
    private static void OnCoroutine(ILContext il)
    {
        ILCursor c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.Before, instr => instr.MatchStfld<Player>("DashDir")))
        {
            c.EmitDup();
            c.EmitDelegate(CheckDownDiag);
        }
    }
    private static void CheckDownDiag(Vector2 dashDir)
    {
        if (!LeniencyHelperModule.Session.Tweaks["NoFailedTech"].Enabled) return;
        
        LeniencyHelperModule.Session.dashCrouched = LeniencyHelperModule.Session.dashCrouched 
            || (dashDir.Y > 0f && dashDir.X != 0f);
    }
    private static void CheckDucking(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);
        if (LeniencyHelperModule.Session.Tweaks["NoFailedTech"].Enabled)
        {
            LeniencyHelperModule.Session.dashCrouched = self.Ducking;
        }
    }
}