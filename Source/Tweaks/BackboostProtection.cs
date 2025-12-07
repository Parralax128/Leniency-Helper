using System;
using Celeste.Mod.LeniencyHelper.Module;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class BackboostProtection : AbstractTweak<BackboostProtection>
{
    private const int EarlyBackboostTiming = 0;
    private const int LateBackboostTiming = 1;

    [OnLoad]
    public static void LoadHooks()
    {
        Everest.Events.Level.OnAfterUpdate += UpdateTimer;
        Everest.Events.Player.OnAfterUpdate += CheckDir;
        On.Celeste.Player.Throw += ConsumeBackboostFacing;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        Everest.Events.Level.OnAfterUpdate -= UpdateTimer;
        Everest.Events.Player.OnAfterUpdate -= CheckDir;
        On.Celeste.Player.Throw -= ConsumeBackboostFacing;
    }

    private static void CheckDir(Player player)
    {
        var s = LeniencyHelperModule.Session;

        float saveDuration;

        if(s.pickupTimeLeft > 0f || player.minHoldTimer > 0f)
        {
            saveDuration = Math.Min(GetSetting<Time>(EarlyBackboostTiming), Player.HoldMinTime + s.pickupTimeLeft);
        }
        else
        {
            saveDuration = GetSetting<Time>(LateBackboostTiming);
        }

        if (player.moveX == 1) s.rightTimer = saveDuration;
        else if (player.moveX == -1) s.leftTimer = saveDuration;

        s.lastFacing = (Facings)Input.MoveX.Value;
    }

    private static void UpdateTimer(Level level)
    {
        var s = LeniencyHelperModule.Session;

        if (s.rightTimer > 0f) s.rightTimer -= Engine.DeltaTime;
        if (s.leftTimer > 0f) s.leftTimer -= Engine.DeltaTime;
    }

    private static void ConsumeBackboostFacing(On.Celeste.Player.orig_Throw orig, Player self)
    {
        if (!Enabled)
        {
            orig(self);
            return;
        }

        if(self.forceMoveXTimer > 0f && !Enabled)
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
