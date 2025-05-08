using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.Module;

public class LeniencyHelperSettings : EverestModuleSettings
{
    private static Dictionary<string, bool?> NulledTweaks()
    {
        Dictionary<string, bool?> result = new Dictionary<string, bool?>();
        foreach (string tweak in TweakList)
            result.Add(tweak, null);
        return result;
    }

    public Dictionary<string, bool?> PlayerTweaks { get; set; } = NulledTweaks();
    public SettingList PlayerSettings { get; set; } = new SettingList();


    [SettingName("LENIENCYHELPER_SETTINGS_SHOWSETTINGS")]
    public bool showSettings { get; set; } = true;
}