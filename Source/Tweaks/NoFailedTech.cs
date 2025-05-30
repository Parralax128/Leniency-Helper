using Celeste.Mod.LeniencyHelper.Module;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class NoFailedTech : AbstractTweak
{
    private static ILHook dashCoroutineHook;

    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Jump += JumpToTech;
        LeniencyHelperModule.OnUpdate += RunTimer;
        On.Celeste.Player.DashEnd += StartProtectionTimer;
        dashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetStateMachineTarget(), GetDashDir);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Jump -= JumpToTech;
        LeniencyHelperModule.OnUpdate -= RunTimer;
        On.Celeste.Player.DashEnd -= StartProtectionTimer;
        dashCoroutineHook.Dispose();
    }

    private static void JumpToTech(On.Celeste.Player.orig_Jump orig, Player self, bool particles = true, bool playSfx = true)
    {
        if (Enabled("NoFailedTech") && self.CanUnDuck && self.DashDir.X != 0
            && self.OnGround() && LeniencyHelperModule.Session.protectedDashAttackTimer > 0f)
        {
            self.Ducking = LeniencyHelperModule.Session.dashCrouched || LeniencyHelperModule.Session.downDiag;
            self.SuperJump();
            return;
        }

        orig(self, particles, playSfx);
    }

    private static void GetDashDir(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("CorrectDashPrecision")))
        {
            cursor.EmitDup();
            cursor.EmitDelegate(SaveDashDir);
        }
    }
    private static void SaveDashDir(Vector2 dir)
    {
        LeniencyHelperModule.Session.downDiag = dir.X != 0f && dir.Y > 0f; 
    }

    private static void StartProtectionTimer(On.Celeste.Player.orig_DashEnd orig, Player self)
    {
        orig(self);

        LeniencyHelperModule.Session.protectedDashAttackTimer = GetSetting<float>("protectedTechTime")
            * (GetSetting<bool>("countProtectedTechTimeInFrames") ? Engine.DeltaTime : 1f);

        LeniencyHelperModule.Session.dashCrouched = self.Ducking;
    }
    private static void RunTimer()
    {
        var s = LeniencyHelperModule.Session;

        if (s.protectedDashAttackTimer > 0f)
            s.protectedDashAttackTimer -= Engine.DeltaTime;
    }
}