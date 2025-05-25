using Celeste.Mod.LeniencyHelper.Module;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class NoFailedTech : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Jump += JumpToTech;
        LeniencyHelperModule.OnUpdate += RunTimer;
        On.Celeste.Player.DashEnd += StartProtectionTimer;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Jump -= JumpToTech;
        LeniencyHelperModule.OnUpdate -= RunTimer;
        On.Celeste.Player.DashEnd -= StartProtectionTimer;
    }

    private static void JumpToTech(On.Celeste.Player.orig_Jump orig, Player self, bool particles = true, bool playSfx = true)
    {
        if (Enabled("NoFailedTech") && self.CanUnDuck && self.DashDir.X != 0
            && self.OnGround() && LeniencyHelperModule.Session.protectedDashAttackTimer > 0f)
        {
            self.Ducking = LeniencyHelperModule.Session.dashCrouched;
            self.SuperJump();
            return;
        }

        orig(self, particles, playSfx);
    }
    private static void StartProtectionTimer(On.Celeste.Player.orig_DashEnd orig, Player self)
    {
        orig(self);
        LeniencyHelperModule.Session.protectedDashAttackTimer = GetSetting<float>("protectedTechTime");
        LeniencyHelperModule.Session.dashCrouched = self.Ducking;
    }
    private static void RunTimer()
    {
        var s = LeniencyHelperModule.Session;

        if (s.protectedDashAttackTimer > 0f)
            s.protectedDashAttackTimer -= Engine.DeltaTime;
    }
}