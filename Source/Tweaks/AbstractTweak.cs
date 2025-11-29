using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class AbstractTweak<TweakType> where TweakType : AbstractTweak<TweakType>
{
    private static readonly Tweak tweak = System.Enum.Parse<Tweak>(typeof(TweakType).Name);
    public static bool Enabled => SettingMaster.GetTweakEnabled(tweak);
    public static T GetSetting<T>(string settingName) => SettingMaster.GetSetting<T>(settingName, tweak);
    public static float GetTime(string settingName) => SettingMaster.GetTime(settingName, tweak);
}