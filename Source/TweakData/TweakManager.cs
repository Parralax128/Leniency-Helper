using Celeste.Mod.LeniencyHelper.TweakData.Settings;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.TweakData;
public static class TweakManager
{
    public static Dictionary<Tweak, TweakState> Tweaks = new()
    {
        { Tweak.AutoSlowfall, new TweakState(Tweak.AutoSlowfall, new SettingContainer {
            new Setting<bool>("techOnly", true),
            new Setting<bool>("delayedJumpRelease", false),
            new Setting<Timer>("releaseDelay", new Timer(0.05f), new Timer(0f), new Timer(1f)),
        }, null) }
    };
}