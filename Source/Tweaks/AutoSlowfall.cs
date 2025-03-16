using Monocle;
using Microsoft.Xna.Framework;
namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class AutoSlowfall
{
    public static void LoadHooks()
    {
        On.Celeste.Player.NormalUpdate += AutoSlowfallOnUpdate;
    }
    public static void UnloadHooks()
    {
        On.Celeste.Player.NormalUpdate -= AutoSlowfallOnUpdate;
    }
    private static int AutoSlowfallOnUpdate(On.Celeste.Player.orig_NormalUpdate orig, Player self)
    {
        if (LeniencyHelperModule.Session.TweaksEnabled["AutoSlowfall"])
            self.AutoJump = true;

        return orig(self);
    }
}