using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CustomBufferTime : AbstractTweak
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


        float mult = GetSetting<bool>("countBufferTimeInFrames") ? Engine.DeltaTime : 1f;

        Input.Jump.BufferTime = GetSetting<float>("JumpBufferTime") * mult;
        Input.Dash.BufferTime = GetSetting<float>("DashBufferTime") * mult;
        Input.CrouchDash.BufferTime = GetSetting<float>("DemoBufferTime") * mult;
    }
    private static void CustomBuffersOnRespawn(On.Celeste.Player.orig_ctor orig, Player self, Vector2 pos, PlayerSpriteMode mode)
    {
        orig(self, pos, mode);

        if (Enabled("CustomBufferTime")) UpdateCustomBuffers();
    }
}