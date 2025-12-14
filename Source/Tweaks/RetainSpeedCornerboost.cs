using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class RetainSpeedCornerboost : AbstractTweak<RetainSpeedCornerboost>
{
    static Timer RetainTimer = new();
    [SaveState] static float speedRetained;


    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.ClimbJump += RetainSpeedOnClimbJump;
        Everest.Events.Player.OnAfterUpdate += RetainTimerUpdate;
        On.Celeste.Player.OnCollideH += SaveCbSpeed;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.ClimbJump -= RetainSpeedOnClimbJump;
        Everest.Events.Player.OnAfterUpdate -= RetainTimerUpdate;
        On.Celeste.Player.OnCollideH -= SaveCbSpeed;
    }

    static void SaveCbSpeed(On.Celeste.Player.orig_OnCollideH orig, Player self, CollisionData data)
    {
        if (!Enabled)
        {
            orig(self, data);
            return;
        }


        float savePlayerSpeed = self.Speed.X;
        orig(self, data);

        if (RetainTimer && Math.Abs(savePlayerSpeed) > 0.1f)
        {
            speedRetained = savePlayerSpeed;
            RetainTimer.Launch(GetSetting<Time>());
        }
    }
    
    static void RetainTimerUpdate(Player player)
    {
        if (!Enabled) return;


        if (RetainTimer && Math.Sign(player.Speed.X) == -Math.Sign(speedRetained)) {
            RetainTimer.Abort();
        }
    }

    static void RetainSpeedOnClimbJump(On.Celeste.Player.orig_ClimbJump orig, Player self)
    {
        if (Enabled && RetainTimer && Math.Abs(speedRetained) > Math.Abs(self.Speed.X))
        {
            self.Speed.X = speedRetained;
            RetainTimer.Abort();
        }

        orig(self);
    }
}