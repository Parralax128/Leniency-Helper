using Monocle;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CustomBufferTime : AbstractTweak<CustomBufferTime>
{
    [OnLoad]
    public static void LoadHooks()
    {
        Everest.Events.Level.OnAfterUpdate += ApplyCustomBuffers;
    }
    [OnUnload]
    public static void UnloadHooks() 
    {
        Everest.Events.Level.OnAfterUpdate -= ApplyCustomBuffers;
    }
    public static void ApplyCustomBuffers(Level level)
    {
        Input.Jump.BufferTime = GetSetting<Time>("JumpBufferTime");
        Input.Dash.BufferTime = GetSetting<Time>("DashBufferTime");
        Input.CrouchDash.BufferTime = GetSetting<Time>("DemoDashBufferTime");
    }
}