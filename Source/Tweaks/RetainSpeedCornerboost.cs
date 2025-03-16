using Monocle;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class RetainSpeedCornerboost
{
    public static void LoadHooks()
    {
        On.Celeste.Player.ClimbJump += RetainSpeedOnClimbJump;
        On.Celeste.Player.Update += DeltaRetainTimerUpdate;
        On.Celeste.Player.OnCollideH += SaveCbSpeed;
    }
    public static void UnloadHooks()
    {
        On.Celeste.Player.ClimbJump -= RetainSpeedOnClimbJump;
        On.Celeste.Player.Update -= DeltaRetainTimerUpdate;
        On.Celeste.Player.OnCollideH -= SaveCbSpeed;
    }

    private static void SaveCbSpeed(On.Celeste.Player.orig_OnCollideH orig, Player self,
        CollisionData data)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["RetainSpeedCornerboost"])
        {
            orig(self, data);
            return;
        }
        var s = LeniencyHelperModule.Session;
        var settings = LeniencyHelperModule.Settings;

        float savePlayerSpeed = self.Speed.X;
        orig(self, data);

        if (s.retainCbSpeedTimer <= 0f && Math.Abs(savePlayerSpeed) > 0.1f)
        {
            s.retainCbSpeed = savePlayerSpeed;
            s.retainCbSpeedTimer = (bool)settings.GetSetting("RetainSpeedCornerboost", "countRetainTimeInFrames") ?
                (float)settings.GetSetting("RetainSpeedCornerboost", "RetainCbSpeedTime") / Engine.DeltaTime :
                (float)settings.GetSetting("RetainSpeedCornerboost", "RetainCbSpeedTime");
        }
    }
    
    private static void DeltaRetainTimerUpdate(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);
        if (!LeniencyHelperModule.Session.TweaksEnabled["RetainSpeedCornerboost"]) return;

        var s = LeniencyHelperModule.Session;

        if (s.retainCbSpeedTimer > 0f)
        {
            if (Math.Sign(self.Speed.X) == -Math.Sign(s.retainCbSpeed))
            {
                s.retainCbSpeedTimer = 0f;
            }
            else
            {
                s.retainCbSpeedTimer -= Engine.DeltaTime;
            }
        }
    }

    private static void RetainSpeedOnClimbJump(On.Celeste.Player.orig_ClimbJump orig, Player self)
    {
        if (LeniencyHelperModule.Session.TweaksEnabled["RetainSpeedCornerboost"])
        {
            var s = LeniencyHelperModule.Session;
            if (s.retainCbSpeedTimer > 0f && Math.Abs(s.retainCbSpeed) > Math.Abs(self.Speed.X))
            {
                self.Speed.X = s.retainCbSpeed;
                s.retainCbSpeedTimer = 0f;
            }
        }

        orig(self);
    }
}