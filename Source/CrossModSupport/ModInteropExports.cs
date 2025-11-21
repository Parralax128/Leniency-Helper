using Celeste.Mod.LeniencyHelper.Module;
using MonoMod.ModInterop;

namespace Celeste.Mod.LeniencyHelper.CrossModSupport;

[ModExportName("LeniencyHelper")]
public static class ModInteropExports
{
    public static bool GetTweakEnabled(string tweakName, bool ignoreOverride = false)
        => SettingMaster.GetTweakEnabled(tweakName, ignoreOverride);

    public static bool GetTweakEnabledByMap(string tweakName)
        => LeniencyHelperModule.Session.UseController[tweakName]
        ? LeniencyHelperModule.Session.ControllerTweaks[tweakName]
        : LeniencyHelperModule.Session.TriggerTweaks[tweakName];

    public static bool GetTweakEnabledByPlayer(string tweakName)
        => LeniencyHelperModule.Settings.PlayerTweaks[tweakName] == true;

    public static bool GetTweakDisabledByPlayer(string tweakName)
        => LeniencyHelperModule.Settings.PlayerTweaks[tweakName] == false;


    public static void SetTweak(string tweakName, bool? state, bool overridePlayerSettings)
    {
        LeniencyHelperModule.Session.OverrideTweaks[tweakName] = state;
        LeniencyHelperModule.Session.OverridePlayerSettings = overridePlayerSettings;
    }   
}