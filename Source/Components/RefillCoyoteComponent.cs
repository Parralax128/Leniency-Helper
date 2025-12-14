namespace Celeste.Mod.LeniencyHelper.Components;

class RefillCoyoteComponent : TweakComponent<Tweaks.RefillDashInCoyote>
{
    public Timer RefillCoyoteTimer = new();
    public override void Update()
    {
        base.Update();
        if (!TweakEnabled) return;

        if (RefillCoyoteTimer && Player.dashRefillCooldownTimer <= 0f)
        {
            Player.RefillDash();
            RefillCoyoteTimer.Abort();
        }
    }

    public void ResetTimer() => RefillCoyoteTimer.Launch(Tweaks.RefillDashInCoyote.GetSetting<Time>());
    public void Cancel() => RefillCoyoteTimer.Abort();
}