using Monocle;
using System;
using static Celeste.Mod.LeniencyHelper.SettingMaster;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public static class CustomDashbounceTiming
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.SuperWallJump += SetCustomTiming;
        On.Celeste.Player.NormalBegin += CustomDashbounce;
        On.Celeste.Player.Update += UpdateTimer;
    }
    [OnUnload]
    public static void UnloadHooks() 
    {
        On.Celeste.Player.SuperWallJump -= SetCustomTiming;
    }

    private static bool justWallbounced = false;
    private static void UpdateTimer(On.Celeste.Player.orig_Update orig, Player self)
    {
        var s = LeniencyHelperModule.Session;
        if (s.dashbounceTimer > 0f) s.dashbounceTimer -= Engine.DeltaTime;

        orig(self);
    }
    private static void CustomDashbounce(On.Celeste.Player.orig_NormalBegin orig, Player self)
    {
        var s = LeniencyHelperModule.Session;
        if (s.Tweaks["CustomDashbounceTiming"].Enabled && !justWallbounced)
        {
            if(s.dashbounceTimer > 0f)
            {
                self.varJumpTimer = Math.Max(self.varJumpTimer, 0.05f);
                s.dashbounceTimer = 0f;
            }
        }
        justWallbounced = false;

        orig(self);
    }
    private static void SetCustomTiming(On.Celeste.Player.orig_SuperWallJump orig, Player self, int dir)
    {
        orig(self, dir);

        if (LeniencyHelperModule.Session.Tweaks["CustomDashbounceTiming"].Enabled)
        {
            LeniencyHelperModule.Session.dashbounceTimer = 0.2f + (GetSetting<bool>("countDashbounceTimingInFrames") ?
                GetSetting<float>("dashbounceTiming") / Engine.FPS : GetSetting<float>("dashbounceTiming"));

            justWallbounced = true;
             
            if(LeniencyHelperModule.Session.dashbounceTimer < self.varJumpTimer)
            {
                self.varJumpTimer = LeniencyHelperModule.Session.dashbounceTimer;
            }
        }
    }
}   
