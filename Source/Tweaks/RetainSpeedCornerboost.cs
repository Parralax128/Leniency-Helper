using Celeste.Mod.LeniencyHelper.Module;
using IL.Celeste;
using Monocle;
using System;
using static Celeste.Mod.LeniencyHelper.SettingMaster;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class RetainSpeedCornerboost : AbstractTweak<RetainSpeedCornerboost>
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.ClimbJump += RetainSpeedOnClimbJump;
        LeniencyHelperModule.OnPlayerUpdate += RetainTimerUpdate;
        On.Celeste.Player.OnCollideH += SaveCbSpeed;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.ClimbJump -= RetainSpeedOnClimbJump;
        LeniencyHelperModule.OnPlayerUpdate -= RetainTimerUpdate;
        On.Celeste.Player.OnCollideH -= SaveCbSpeed;
    }

    private static void SaveCbSpeed(On.Celeste.Player.orig_OnCollideH orig, Player self,
        CollisionData data)
    {
        if (!Enabled)
        {
            orig(self, data);
            return;
        }
        var s = LeniencyHelperModule.Session;

        float savePlayerSpeed = self.Speed.X;
        orig(self, data);

        if (s.retainCbSpeedTimer <= 0f && Math.Abs(savePlayerSpeed) > 0.1f)
        {
            s.retainCbSpeed = savePlayerSpeed;
            s.retainCbSpeedTimer = GetSetting<bool>("countRetainTimeInFrames") ?
                GetSetting<float>("RetainCbSpeedTime") / Engine.DeltaTime : GetSetting<float>("RetainCbSpeedTime");
        }
    }
    
    private static void RetainTimerUpdate(Player player)
    {
        if (!Enabled) return;

        var s = LeniencyHelperModule.Session;

        if (s.retainCbSpeedTimer > 0f)
        {
            if (Math.Sign(player.Speed.X) == -Math.Sign(s.retainCbSpeed))
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
        if (Enabled)
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