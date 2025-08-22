using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class AbstractTweak<TweakType> where TweakType : AbstractTweak<TweakType>
{
    private static readonly string tweakName = typeof(TweakType).Name;
    public static bool Enabled => SettingMaster.GetTweakEnabled(tweakName);
    public static T GetSetting<T>(string settingName) => SettingMaster.GetSetting<T>(settingName, tweakName);
    public static float GetTime(string timeName) => SettingMaster.GetTime(timeName, tweakName);

    public static void Log(object var) => LeniencyHelperModule.Log(var);
}