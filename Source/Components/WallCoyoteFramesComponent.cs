using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.Tweaks;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

public class WallCoyoteFramesComponent : PlayerComponent
{
    private float WallCoyoteTime => SettingMaster.GetSetting<bool>("countWallCoyoteTimeInFrames", Tweak.WallCoyoteFrames) ?
        SettingMaster.GetSetting<float>("wallCoyoteTime", Tweak.WallCoyoteFrames) / Engine.FPS
        : SettingMaster.GetSetting<float>("wallCoyoteTime", Tweak.WallCoyoteFrames);
    
    public float wallCoyoteTimer;
    public WallCoyoteFrames.WallCoyoteTypes currentWallCoyoteType;

    public WallCoyoteFramesComponent() : base(Tweak.WallCoyoteFrames)
    {
        if (Entity is not Player) RemoveSelf();
        wallCoyoteTimer = 0f;
        currentWallCoyoteType = WallCoyoteFrames.WallCoyoteTypes.Right;
    }

    public override void Update()
    {
        base.Update();
        if (!SettingMaster.GetTweakEnabled(Tweak.WallCoyoteFrames) || LeniencyHelperModule.Session == null) return;

        if (wallCoyoteTimer > 0f) wallCoyoteTimer -= Engine.DeltaTime;
        else wallCoyoteTimer = 0f;

        WallCoyoteFrames.useOrigWJCheck = true;
        bool wallLeft = (Entity as Player).WallJumpCheck(-1);
        bool wallRight = (Entity as Player).WallJumpCheck(1);
        WallCoyoteFrames.useOrigWJCheck = false;

        if (wallLeft && wallRight)
        {
            wallCoyoteTimer = WallCoyoteTime;
            currentWallCoyoteType = WallCoyoteFrames.WallCoyoteTypes.Both;
        }
        else if (wallLeft && !wallRight)
        {
            wallCoyoteTimer = WallCoyoteTime;
            currentWallCoyoteType = WallCoyoteFrames.WallCoyoteTypes.Right;
        }
        else if (wallRight && !wallLeft)
        {
            wallCoyoteTimer = WallCoyoteTime;
            currentWallCoyoteType = WallCoyoteFrames.WallCoyoteTypes.Left;
        }
    }
}