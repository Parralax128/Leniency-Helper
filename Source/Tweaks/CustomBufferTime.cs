using Monocle;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CustomBufferTime
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.ctor += CustomBuffersOnRespawn;
    }
    [OnUnload]
    public static void UnloadHooks() 
    {
        On.Celeste.Player.ctor -= CustomBuffersOnRespawn;
    }
    public static void UpdateCustomBuffers()
    {
        bool timeInFrames = SettingMaster.GetSetting<bool>("countBufferTimeInFrames");
        Input.Jump.BufferTime = SettingMaster.GetSetting<float>("JumpBufferTime") * (timeInFrames ? Engine.DeltaTime : 1f);
        Input.Dash.BufferTime = SettingMaster.GetSetting<float>("DashBufferTime") * (timeInFrames ? Engine.DeltaTime : 1f);
        Input.CrouchDash.BufferTime = SettingMaster.GetSetting<float>("DemoBufferTime") * (timeInFrames ? Engine.DeltaTime : 1f);
        Log($"updated buffers: {Input.Jump.BufferTime} {Input.Dash.BufferTime} {Input.CrouchDash.BufferTime}");
    }
    private static void CustomBuffersOnRespawn(On.Celeste.Player.orig_ctor orig, Player self, Vector2 pos, PlayerSpriteMode mode)
    {
        orig(self, pos, mode);

        if (LeniencyHelperModule.Session.Tweaks["CustomBufferTime"].Enabled) UpdateCustomBuffers();
    }
}