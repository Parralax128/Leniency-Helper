using Monocle;
using static Celeste.Mod.LeniencyHelper.SettingMaster;

namespace Celeste.Mod.LeniencyHelper.Components;

public class RefillCoyoteComponent : Component
{
    public static float RefillCoyoteTime => GetSetting<bool>("CountRefillCoyoteTimeInFrames") ?
        GetSetting<float>("RefillCoyoteTime") / Engine.FPS : GetSetting<float>("RefillCoyoteTime");

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

    public void Cancel()
    {
        refillCoyoteTimer = 0f;
    }
}