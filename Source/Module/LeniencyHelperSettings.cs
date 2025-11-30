using System.Collections.Generic;
using System.Linq;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.Module;

public class LeniencyHelperSettings : EverestModuleSettings
{
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