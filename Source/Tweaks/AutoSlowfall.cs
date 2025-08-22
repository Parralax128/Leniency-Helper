
namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class AutoSlowfall : AbstractTweak<AutoSlowfall>
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.NormalUpdate += AutoSlowfallOnUpdate;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.NormalUpdate -= AutoSlowfallOnUpdate;
    }

    private static int AutoSlowfallOnUpdate(On.Celeste.Player.orig_NormalUpdate orig, Player self)
    {
        if (Enabled) self.AutoJump = true;
        return orig(self);
    }
}   