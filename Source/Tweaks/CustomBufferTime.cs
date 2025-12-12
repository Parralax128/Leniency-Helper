using Monocle;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class CustomBufferTime : AbstractTweak<CustomBufferTime>
{
    [SettingIndex] static int JumpBufferTime;
    [SettingIndex] static int DashBufferTime;
    [SettingIndex] static int DemoDashBufferTime;

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
        Input.Jump.BufferTime = GetSetting<Time>(JumpBufferTime);
        Input.Dash.BufferTime = GetSetting<Time>(DashBufferTime);
        Input.CrouchDash.BufferTime = GetSetting<Time>(DemoDashBufferTime);
    }
}