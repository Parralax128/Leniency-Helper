using Monocle;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class NoFailedTech
{
    public static void LoadHooks()
    {
        On.Celeste.Player.Jump += JumpToHyper;
        On.Celeste.Player.DashBegin += CheckDucking;
        On.Celeste.Player.OnCollideV += ProtectDashAttack;
        On.Celeste.Player.Update += ChangeProtectedDashAttack;
    }
    public static void UnloadHooks()
    {
        On.Celeste.Player.Jump -= JumpToHyper;
        On.Celeste.Player.DashBegin -= CheckDucking;
        On.Celeste.Player.OnCollideV -= ProtectDashAttack;
        On.Celeste.Player.Update -= ChangeProtectedDashAttack;
    }

    private static void JumpToHyper(On.Celeste.Player.orig_Jump orig, Player self, bool particles = true, bool playSfx = true)
    {
        if (LeniencyHelperModule.Session.TweaksEnabled["NoFailedTech"] && self.CanUnDuck &&
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
        if (LeniencyHelperModule.Session.TweaksEnabled["NoFailedTech"])
            LeniencyHelperModule.Session.protectedDashAttackTimer = self.dashAttackTimer;

        orig(self, data);            
    }
    private static void CheckDucking(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);

        if (LeniencyHelperModule.Session.TweaksEnabled["NoFailedTech"])
            LeniencyHelperModule.Session.dashCrouched = self.Ducking;
    }
}