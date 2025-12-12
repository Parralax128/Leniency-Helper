using Celeste.Mod.LeniencyHelper.Module;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

class WallCoyoteFramesComponent : PlayerComponent
{
    public float wallCoyoteTimer;
    public Tweaks.WallCoyoteFrames.WallCoyoteTypes currentWallCoyoteType;

    public WallCoyoteFramesComponent() : base(Tweak.WallCoyoteFrames)
    {
        if (Entity is not Player) RemoveSelf();
        wallCoyoteTimer = 0f;
        currentWallCoyoteType = Tweaks.WallCoyoteFrames.WallCoyoteTypes.Right;
    }

    public override void Update()
    {
        base.Update();
        if (!TweakData.Tweaks[Tweak.WallCoyoteFrames].Enabled || LeniencyHelperModule.Session == null) return;

        if (wallCoyoteTimer > 0f) wallCoyoteTimer -= Engine.DeltaTime;
        else wallCoyoteTimer = 0f;

        Tweaks.WallCoyoteFrames.useOrigWJCheck = true;
        bool wallLeft = (Entity as Player).WallJumpCheck(-1);
        bool wallRight = (Entity as Player).WallJumpCheck(1);
        Tweaks.WallCoyoteFrames.useOrigWJCheck = false;

        if (wallLeft && wallRight)
        {
            wallCoyoteTimer = Tweaks.WallCoyoteFrames.GetSetting<Time>();
            currentWallCoyoteType = Tweaks.WallCoyoteFrames.WallCoyoteTypes.Both;
        }
        else if (wallLeft && !wallRight)
        {
            wallCoyoteTimer = Tweaks.WallCoyoteFrames.GetSetting<Time>();
            currentWallCoyoteType = Tweaks.WallCoyoteFrames.WallCoyoteTypes.Right;
        }
        else if (wallRight && !wallLeft)
        {
            wallCoyoteTimer = Tweaks.WallCoyoteFrames.GetSetting<Time>();
            currentWallCoyoteType = Tweaks.WallCoyoteFrames.WallCoyoteTypes.Left;
        }
    }
}