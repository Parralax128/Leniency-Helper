using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

public class WallCoyoteFramesComponent : Component
{
    public float wallCoyoteTime => LeniencyHelperModule.Settings is null ? 0f : SettingMaster.GetSetting<float>("wallCoyoteTime");
    public float wallCoyoteTimer => LeniencyHelperModule.Session is null? 0f : LeniencyHelperModule.Session.wallCoyoteTimer;
    public enum WallCoyoteTypes
    {
        Left,
        Right,
        Both
    }

    public WallCoyoteTypes currentWallCoyoteType;
    public override void Update()
    {
        base.Update();

        if (!LeniencyHelperModule.Session.Tweaks["WallCoyoteFrames"].Enabled)
        {
            LeniencyHelperModule.Session.prevWallCoyoteTime = wallCoyoteTime;
        }
    }
    public WallCoyoteFramesComponent() : base(true, true) { }
}