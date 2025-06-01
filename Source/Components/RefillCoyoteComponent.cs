using Celeste.Mod.LeniencyHelper.Tweaks;
using Monocle;
using static Celeste.Mod.LeniencyHelper.SettingMaster;

namespace Celeste.Mod.LeniencyHelper.Components;

public class RefillCoyoteComponent : PlayerComponent
{
    public static float RefillCoyoteTime => GetSetting<bool>("CountRefillCoyoteTimeInFrames", "RefillDashInCoyote") ?
        GetSetting<float>("RefillCoyoteTime", "RefillDashInCoyote") * Engine.DeltaTime : GetSetting<float>("RefillCoyoteTime", "RefillDashInCoyote");

    public float refillCoyoteTimer;
    public RefillCoyoteComponent() : base("RefillDashInCoyote") 
    {
        refillCoyoteTimer = 0f;
    }
    public override void Update()
    {
        base.Update();
        if (!Tweaks.AbstractTweak.Enabled("RefillDashInCoyote")) return;

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

    public void Cancel()
    {
        refillCoyoteTimer = 0f;
    }
}