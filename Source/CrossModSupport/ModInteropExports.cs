using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.TweakSettings;
using MonoMod.ModInterop;
using System;

namespace Celeste.Mod.LeniencyHelper.CrossModSupport;

[ModExportName("LeniencyHelper")]
public static class ModInteropExports
{
    private static TweakState Parse(string tweakName) => TweakData.Tweaks[Enum.Parse<Tweak>(tweakName)];
    public static bool GetTweakEnabled(string tweakName, bool ignoreOverride = false)
    {
        TweakState tweakState = Parse(tweakName);
        if(ignoreOverride)
        {
            bool? savedValue = tweakState.Get(SettingSource.API);
            tweakState.Set(null, SettingSource.API);
            bool result = tweakState.Enabled;

            tweakState.Set(savedValue, SettingSource.API);
            return result;
        }

        return tweakState.Enabled;
    }

    public static bool GetTweakEnabledByMap(string tweakName)
    {
        TweakState tweakState = Parse(tweakName);
        return tweakState.Get(SettingSource.Trigger) ?? tweakState.Get(SettingSource.Controller) == true;
    }

    public static bool GetTweakEnabledByPlayer(string tweakName)
        => Parse(tweakName).Get(SettingSource.Player) == true;

    public static bool GetTweakDisabledByPlayer(string tweakName)
        => Parse(tweakName).Get(SettingSource.Player) == false;

    public static void SetTweak(string tweakName, bool? state, bool overridePlayerSettings)
    {
        TweakState tweakState = Parse(tweakName);

        tweakState.Set(state, SettingSource.API);
    }
}