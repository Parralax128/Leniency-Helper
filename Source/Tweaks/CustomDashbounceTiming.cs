using Celeste.Mod.LeniencyHelper.Module;
using Monocle;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CustomDashbounceTiming : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.SuperWallJump += SetCustomTiming;
        On.Celeste.Player.NormalBegin += DashbounceIfCan;
        On.Celeste.Player.Update += UpdateTimer;
        On.Celeste.Player.DashBegin += TimerCheck;
    }
    [OnUnload]
    public static void UnloadHooks() 
    {
        On.Celeste.Player.SuperWallJump -= SetCustomTiming;
    }

    private static void SetCustomTiming(On.Celeste.Player.orig_SuperWallJump orig, Player self, int dir)
    {
        orig(self, dir);
        if (!Enabled("CustomDashbounceTiming")) return;

        LeniencyHelperModule.Session.dashbounceTimer = GetSetting<float>("dashbounceTiming") *
            (GetSetting<bool>("countDashbounceTimingInFrames") ? Engine.DeltaTime : 1f); ;
    }
    private static void UpdateTimer(On.Celeste.Player.orig_Update orig, Player self)
    {
        if (LeniencyHelperModule.Session.dashbounceTimer > 0f)
        {
            LeniencyHelperModule.Session.dashbounceTimer -= Engine.DeltaTime;
        }

        orig(self);
    }
    private static void TimerCheck(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);
        if (LeniencyHelperModule.Session.dashbounceTimer > 0f)
        {
            LeniencyHelperModule.Session.canDashbounce = true;
            LeniencyHelperModule.Session.dashbounceTimer = 0f;
        }
        else
        {
            LeniencyHelperModule.Session.canDashbounce = false;
        }
    }
    private static void DashbounceIfCan(On.Celeste.Player.orig_NormalBegin orig, Player self)
    {
        if (LeniencyHelperModule.Session.canDashbounce)
        {
            self.varJumpTimer = Math.Max(self.varJumpTimer, 0.05f);
            LeniencyHelperModule.Session.canDashbounce = false;
        }
        else
        {
            self.varJumpTimer = 0f;
        }

        orig(self);
    }

}   