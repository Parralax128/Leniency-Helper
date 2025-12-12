using Monocle;
using Microsoft.Xna.Framework;
using System;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class DashCDIgnoreFFrames : AbstractTweak<DashCDIgnoreFFrames>
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Celeste.Freeze += UpdateDashCDOnFreeze;
        On.Celeste.Player.Added += GetPlayerOnAdded;
        On.Celeste.Player.DashBegin += UseDefaultFFOnDash;
        On.Celeste.Player.DashUpdate += InstantDashBegin;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Celeste.Freeze -= UpdateDashCDOnFreeze;
        On.Celeste.Player.Added -= GetPlayerOnAdded;
        On.Celeste.Player.DashBegin -= UseDefaultFFOnDash;
        On.Celeste.Player.DashUpdate -= InstantDashBegin;
    }
    

    public static void UseDefaultFFOnDash(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        var s = LeniencyHelperModule.Session;
        s.useOrigFreeze = true;
        orig(self);
        s.useOrigFreeze = false;
    }   
    public static void GetPlayerOnAdded(On.Celeste.Player.orig_Added orig, Player self, Scene scene)
    {
        var s = LeniencyHelperModule.Session;
        orig(self, scene);
        s.modifiedPlayer = self;
    }
    public static void UpdateDashCDOnFreeze(On.Celeste.Celeste.orig_Freeze orig, float time)
    {
        var s = LeniencyHelperModule.Session;
        orig(time);
        if(!Enabled || s.useOrigFreeze) return;
        {
            if (s.modifiedPlayer != null)
                s.modifiedPlayer.dashCooldownTimer -= time;
        }
    }

    public static int InstantDashBegin(On.Celeste.Player.orig_DashUpdate orig, Player self)
    {
        if (!Enabled) return orig(self);

        if (self.CanDash)
        {
            self.StartDash();
            self.StateMachine.ForceState(2);
            return 2;
        }

        return orig(self);
    }
}