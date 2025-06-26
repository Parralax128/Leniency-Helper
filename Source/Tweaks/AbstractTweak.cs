using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public abstract class AbstractTweak
{
    public static bool Enabled(string tweak)
    {
        if (LeniencyHelperModule.Settings.PlayerTweaks[tweak].HasValue)
            return LeniencyHelperModule.Settings.PlayerTweaks[tweak].Value;

        return LeniencyHelperModule.Session.UseController[tweak] ?
            LeniencyHelperModule.Session.ControllerTweaks[tweak] : LeniencyHelperModule.Session.TriggerTweaks[tweak];   
    }
    public static T GetSetting<T>(string name)
    {
        string tweak = "null";
        foreach(string key in SettingMaster.AssociatedSettings.Keys)
        {
            if (SettingMaster.AssociatedSettings[key] == null) continue;
            if (SettingMaster.AssociatedSettings[key].Contains(name))
            {
                tweak = key;
                break;
            }
        }
        return SettingMaster.GetSetting<T>(name, tweak);
    }

    public static void Log(object var) => LeniencyHelperModule.Log(var);
}