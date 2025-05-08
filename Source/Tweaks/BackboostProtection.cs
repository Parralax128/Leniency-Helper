using Celeste.Mod.LeniencyHelper.Module;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

static  class BackboostProtection
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Throw += OnThrow;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Throw -= OnThrow;
    }



    private static void OnThrow(On.Celeste.Player.orig_Throw orig, Player self)
    {
        orig(self);
        LeniencyHelperModule.Log("throw!");
    }
}
