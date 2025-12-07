using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class AbstractTweak<TweakType> where TweakType : AbstractTweak<TweakType>
{
    private static readonly Tweak tweak = System.Enum.Parse<Tweak>(typeof(TweakType).Name);
    public static bool Enabled => TweakData.Tweaks[tweak].Enabled;
    public static T GetSetting<T>(int index) => TweakData.Tweaks[tweak].GetSetting<T>(index);
    public static T GetTemp<T>(string tempName) => TweakData.Tweaks[tweak].GetTemp<T>(tempName);
    public static void SetTemp(string tempName, object value) => TweakData.Tweaks[tweak].SetTemp(tempName, value);
}