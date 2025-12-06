using Celeste.Mod.LeniencyHelper.UI;

namespace Celeste.Mod.LeniencyHelper;

public static class Debug
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
        if (Input.Grab.Check)
            Logger.Info("LeniencyHelper", o?.ToString() ?? "null");
    }
    public static void GrabWarn(object o)
    {
        if (Input.Grab.Check)
            Logger.Warn("LeniencyHelper", o?.ToString() ?? "null");
    }
}
