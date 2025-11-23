using Celeste.Mod.LeniencyHelper.UI;

namespace Celeste.Mod.LeniencyHelper;

public class Logging
{
    public static void Log(object o)
    {
        Logger.Info("LeniencyHelper", o?.ToString() ?? "null");
    }
    public static void Warn(object o)
    {
        Logger.Warn("LeniencyHelper", o?.ToString() ?? "null");
    }

    public static void GrabLog(object o)
    {
        if (Input.Grab.Check) Log(o);
    }
    public static void GrabWarn(object o)
    {
        if (Input.Grab.Check) Warn(o);
    }

    public static void Log(WebScrapper.TweakInfo info)
    {
        Warn(info.description);
        if (info.settingDescs == null) return;
        foreach (string key in info.settingDescs.Keys)
        {
            Log($"{key}: {info.settingDescs[key]}");
        }
    }
}
