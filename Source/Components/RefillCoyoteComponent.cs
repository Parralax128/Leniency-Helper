using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

public class RefillCoyoteComponent : Component
{
    public static float RefillCoyoteTime =>
    ((bool)LeniencyHelperModule.Settings.GetSetting("RefillDashInCoyote", "CountRefillCoyoteTimeInFrames") ?
    (float)LeniencyHelperModule.Settings.GetSetting("RefillDashInCoyote", "RefillCoyoteTime") / Engine.FPS :
    (float)LeniencyHelperModule.Settings.GetSetting("RefillDashInCoyote", "RefillCoyoteTime"));

    public float refillCoyoteTimer;
    public RefillCoyoteComponent() : base(true, true) 
    {
        refillCoyoteTimer = 0f;
    }
    public override void Update()
    {
        base.Update();

        Player player = Entity as Player;
        if (refillCoyoteTimer > 0f)
        {
            refillCoyoteTimer -= Engine.DeltaTime;
            if (player.dashRefillCooldownTimer <= 0f)
            {
                player.RefillDash();
                refillCoyoteTimer = 0f;
            }
        }
    }
    public void ResetTimer()
    {
        refillCoyoteTimer = RefillCoyoteTime;
    }
}