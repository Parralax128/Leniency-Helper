using Celeste.Mod.LeniencyHelper.Module;
using MonoMod.ModInterop;
using static System.Enum;

namespace Celeste.Mod.LeniencyHelper.CrossModSupport;

[ModExportName("LeniencyHelper")]
public static class ModInteropExports
{
    public static bool GetTweakEnabled(string tweakName, bool ignoreOverride = false)
        => SettingMaster.GetTweakEnabled(Parse<Tweak>(tweakName), ignoreOverride);

    public static bool GetTweakEnabledByMap(string tweakName)
    {
        Tweak tweak = Parse<Tweak>(tweakName);
        return LeniencyHelperModule.Session.UseController[tweak]
        ? LeniencyHelperModule.Session.ControllerTweaks[tweak]
        : LeniencyHelperModule.Session.TriggerTweaks[tweak];
    }
        

    public static bool GetTweakEnabledByPlayer(string tweakName)
        => LeniencyHelperModule.Settings.PlayerTweaks[Parse<Tweak>(tweakName)] == true;

    public static bool GetTweakDisabledByPlayer(string tweakName)
        => LeniencyHelperModule.Settings.PlayerTweaks[Parse<Tweak>(tweakName)] == false;


    public static void SetTweak(string tweakName, bool? state, bool overridePlayerSettings)
    {
        LeniencyHelperModule.Session.OverrideTweaks[Parse<Tweak>(tweakName)] = state;
        LeniencyHelperModule.Session.OverridePlayerSettings = overridePlayerSettings;
    }   
}