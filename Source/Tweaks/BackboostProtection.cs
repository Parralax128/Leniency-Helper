using System;
using Celeste.Mod.LeniencyHelper.Module;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class BackboostProtection : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        LeniencyHelperModule.OnUpdate += UpdateTimer;
        LeniencyHelperModule.OnPlayerUpdate += CheckDir;
        On.Celeste.Player.Throw += ConsumeBackboostFacing;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        LeniencyHelperModule.OnUpdate -= UpdateTimer;
        LeniencyHelperModule.OnPlayerUpdate -= CheckDir;
        On.Celeste.Player.Throw -= ConsumeBackboostFacing;
    }

    private static void CheckDir(Player player)
    {
        var s = LeniencyHelperModule.Session;

        float saveDuration;

        if(s.pickupTimeLeft > 0f || player.minHoldTimer > 0f)
        {
            saveDuration = Math.Min( 
                GetSetting<float>("earlyBackboostTiming")
                    * (GetSetting<bool>("countBackboostTimingInFrames") ? Engine.DeltaTime : 1f),
                Player.HoldMinTime + s.pickupTimeLeft);
        }
        else
        {
            saveDuration = GetSetting<float>("lateBackboostTiming")
                * (GetSetting<bool>("countBackboostTimingInFrames") ? Engine.DeltaTime : 1f);
        }

        if (player.moveX == 1) s.rightTimer = saveDuration;
        else if (player.moveX == -1) s.leftTimer = saveDuration;

        s.lastFacing = (Facings)Input.MoveX.Value;
    }

    private static void UpdateTimer()
    {
        var s = LeniencyHelperModule.Session;

        if (s.rightTimer > 0f) s.rightTimer -= Engine.DeltaTime;
        if (s.leftTimer > 0f) s.leftTimer -= Engine.DeltaTime;
    }

    private static void ConsumeBackboostFacing(On.Celeste.Player.orig_Throw orig, Player self)
    {
        if (!Enabled("BackboostProtection"))
        {
            orig(self);
            return;
        }

        if(self.forceMoveXTimer > 0f && !Enabled("DisableForcemovedTech"))
        {
            orig(self);
            return;
        }

        Facings? savedPlayerFacing = null;
        int? savedMoveY = null;

        var s = LeniencyHelperModule.Session;

        if(Math.Sign(self.Speed.X) == -1)
        {
            if(s.rightTimer > 0f)
            {
                savedPlayerFacing = self.Facing;
                self.Facing = Facings.Right;

                savedMoveY = Input.MoveY.Value;
                Input.MoveY.Value = 0;
            }
        }
        else if(Math.Sign(self.Speed.X) == 1)
        {
            if (s.leftTimer > 0f)
            {
                savedPlayerFacing = self.Facing;
                self.Facing = Facings.Left;

                savedMoveY = Input.MoveY.Value;
                Input.MoveY.Value = 0;
            }
        }
        else
        {
            if (self.Facing == Facings.Left)
            {
                if (s.rightTimer > 0f)
                {
                    savedPlayerFacing = self.Facing;
                    self.Facing = Facings.Right;

                    savedMoveY = Input.MoveY.Value;
                    Input.MoveY.Value = 0;
                }
            }
            else if (self.Facing == Facings.Right)
            {
                if (s.leftTimer > 0f)
                {
                    savedPlayerFacing = self.Facing;
                    self.Facing = Facings.Left;

                    savedMoveY = Input.MoveY.Value;
                    Input.MoveY.Value = 0;
                }
            }
        }

        orig(self);

        if (savedPlayerFacing.HasValue) self.Facing = savedPlayerFacing.Value;
        if (savedMoveY.HasValue) Input.MoveY.Value = savedMoveY.Value;
    }
}
