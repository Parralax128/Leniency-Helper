using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper;

public class LeniencyHelperModuleSettings : EverestModuleSettings 
{
    public Dictionary<string, bool?> SavedPlayerTweaks { get; set; } = GetDefaultSettings();
    public static Dictionary<string, bool?> GetDefaultSettings()
    {
        Dictionary<string, bool?> dict = new Dictionary<string, bool?>();
        foreach(string tweak in tweakList)
        {
            dict.Add(tweak, null);
        }
        return dict;
    }

    public Dictionary<string, object> SavedTweakSettings { get; set; } = GetDefaultPairs();
    private static Dictionary<string, object> GetDefaultPairs()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        foreach(string setting in SettingMaster.TweakSettings.Keys)
        {
            result.Add(setting, SettingMaster.GetDefaultSetting(setting));
        }
        return result;
    }

    [SettingName("LENIENCYHELPER_SETTINGS_SHOWSETTINGS")]
    public bool showSettings { get; set; } = true;
}