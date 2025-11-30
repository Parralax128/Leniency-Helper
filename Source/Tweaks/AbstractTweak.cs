using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class AbstractTweak<TweakType> where TweakType : AbstractTweak<TweakType>
{
    private static readonly Tweak tweak = System.Enum.Parse<Tweak>(typeof(TweakType).Name);
    public static bool Enabled => TweakData.Tweaks[tweak].Enabled;
    public static T GetSetting<T>(string settingName) => TweakData.Tweaks[tweak].GetSetting<T>(settingName);
}