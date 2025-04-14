using Monocle;
using Microsoft.Xna.Framework;
using System;
namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class AutoSlowfall
{
    [OnLoad]
    public static void LoadHooks()
    {
        Console.WriteLine("autoslowfall Loaded hooks");
        On.Celeste.Player.NormalUpdate += AutoSlowfallOnUpdate;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.NormalUpdate -= AutoSlowfallOnUpdate;
    }
    private static int AutoSlowfallOnUpdate(On.Celeste.Player.orig_NormalUpdate orig, Player self)
    {
        if (LeniencyHelperModule.Session.Tweaks["AutoSlowfall"].Enabled)
            self.AutoJump = true;

        return orig(self);
    }
}