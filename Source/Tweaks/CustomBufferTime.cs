using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CustomBufferTime : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        LeniencyHelperModule.OnUpdate += ApplyCustomBuffers;
    }
    [OnUnload]
    public static void UnloadHooks() 
    {
        LeniencyHelperModule.OnUpdate -= ApplyCustomBuffers;
    }
    public static void ApplyCustomBuffers()
    {
        float mult = GetSetting<bool>("countBufferTimeInFrames") ? Engine.DeltaTime : 1f;
        Input.Jump.BufferTime = GetSetting<float>("JumpBufferTime") * mult;
        Input.Dash.BufferTime = GetSetting<float>("DashBufferTime") * mult;
        Input.CrouchDash.BufferTime = GetSetting<float>("DemoBufferTime") * mult;
    }
}