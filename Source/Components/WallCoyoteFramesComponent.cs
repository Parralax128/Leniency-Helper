namespace Celeste.Mod.LeniencyHelper.Components;

class WallCoyoteFramesComponent : TweakComponent<Tweaks.WallCoyoteFrames>
{
    Timer timer = new();
    Timer disableTimer = new(0.09f);
    Tweaks.WallCoyoteFrames.WallSides side = 0;

    public override void Update()
    {
        base.Update();
        if (!TweakEnabled || disableTimer) return;


        Tweaks.WallCoyoteFrames.useOrigWJCheck = true;
        bool wallLeft = Player.WallJumpCheck(-1);
        bool wallRight = Player.WallJumpCheck(1);
        Tweaks.WallCoyoteFrames.useOrigWJCheck = false;

        if(wallLeft || wallRight)
        {
            timer.Launch(Tweaks.WallCoyoteFrames.GetSetting<Time>());
            side = (Tweaks.WallCoyoteFrames.WallSides)(FromBool(wallRight) - FromBool(wallLeft));
        }

        static int FromBool(bool value) => value ? 1 : 0;
    }

    public bool CheckWall(int dir) => -dir != (int)side && timer;
    public bool NullHorizontal => side == Tweaks.WallCoyoteFrames.WallSides.Both && timer;
    public void AbortTimer()
    {
        timer.Abort();
        disableTimer.Launch();
    }
} 