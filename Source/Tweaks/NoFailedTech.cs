using Celeste.Mod.LeniencyHelper.Module;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class NoFailedTech : AbstractTweak<NoFailedTech>
{
    static ILHook dashCoroutineHook;

    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Jump += JumpToTech;
        Everest.Events.Level.OnAfterUpdate += RunTimer;
        On.Celeste.Player.DashEnd += StartProtectionTimer;
        dashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetStateMachineTarget(), GetDashDir);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Jump -= JumpToTech;
        Everest.Events.Level.OnAfterUpdate -= RunTimer;
        On.Celeste.Player.DashEnd -= StartProtectionTimer;
        dashCoroutineHook.Dispose();
    }

    static void JumpToTech(On.Celeste.Player.orig_Jump orig, Player self, bool particles = true, bool playSfx = true)
    {
        if (Enabled && self.CanUnDuck && self.DashDir.X != 0
            && self.OnGround() && LeniencyHelperModule.Session.protectedDashAttackTimer > 0f)
        {
            self.Ducking = LeniencyHelperModule.Session.dashCrouched || LeniencyHelperModule.Session.downDiag;
            self.SuperJump();
            return;
        }

        orig(self, particles, playSfx);
    }

    static void GetDashDir(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("CorrectDashPrecision")))
        {
            cursor.EmitDup();
            cursor.EmitDelegate(SaveDashDir);
        }
    }
    static void SaveDashDir(Vector2 dir)
    {
        LeniencyHelperModule.Session.downDiag = dir.X != 0f && dir.Y > 0f; 
    }

    static void StartProtectionTimer(On.Celeste.Player.orig_DashEnd orig, Player self)
    {
        orig(self);

        LeniencyHelperModule.Session.protectedDashAttackTimer = GetSetting<Time>();
        LeniencyHelperModule.Session.dashCrouched = self.Ducking;
    }
    static void RunTimer(Level level)
    {
        var s = LeniencyHelperModule.Session;

        if (s.protectedDashAttackTimer > 0f)
            s.protectedDashAttackTimer -= Engine.DeltaTime;
    }
}