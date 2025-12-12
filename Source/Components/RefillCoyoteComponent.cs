using Monocle;
using static Celeste.Mod.LeniencyHelper.SettingMaster;

namespace Celeste.Mod.LeniencyHelper.Components;

class RefillCoyoteComponent : PlayerComponent
{
    public float refillCoyoteTimer;
    public RefillCoyoteComponent() : base(Tweak.RefillDashInCoyote) 
    {
        refillCoyoteTimer = 0f;
    }
    public override void Update()
    {
        base.Update();
        if (!Tweaks.RefillDashInCoyote.Enabled) return;

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
        refillCoyoteTimer = Tweaks.RefillDashInCoyote.GetSetting<Time>();
    }

    public void Cancel()
    {
        refillCoyoteTimer = 0f;
    }
}