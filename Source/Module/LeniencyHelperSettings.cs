using System.Collections.Generic;
using System.Linq;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.Module;

public class LeniencyHelperSettings : EverestModuleSettings
{
    public static Dictionary<Tweak, bool?> NulledTweaks()
    {
        Dictionary<Tweak, bool?> result = new Dictionary<Tweak, bool?>();
        foreach (Tweak tweak in TweakList)
            result.Add(tweak, null);
        return result;
    }

    public Dictionary<Tweak, bool?> PlayerTweaks { get; set; } = NulledTweaks();
    public SettingList PlayerSettings { get; set; } = new SettingList();


    [SettingName("LENIENCYHELPER_SETTINGS_SHOWSETTINGS")]
    public bool showSettings { get; set; } = true;

    public enum UrlActions
    {
        OpenInBrowser,
        CopyToClipboard
    }

    [SettingName("LENIENCYHELPER_SETTINGS_LINKOPENINGMODE")]
    public UrlActions LinkOpeningMode { get; set; } = UrlActions.OpenInBrowser;
}