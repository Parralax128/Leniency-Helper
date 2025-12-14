using System;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class BackboostProtection : AbstractTweak<BackboostProtection>
{
    [SettingIndex] static int EarlyBackboostTiming;
    [SettingIndex] static int LateBackboostTiming;
    
    static Timer LeftTimer = new();
    static Timer RightTimer = new();


    [OnLoad]
    public static void LoadHooks()
    {
        Everest.Events.Player.OnAfterUpdate += CheckDir;
        On.Celeste.Player.Throw += ConsumeBackboostFacing;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        Everest.Events.Player.OnAfterUpdate -= CheckDir;
        On.Celeste.Player.Throw -= ConsumeBackboostFacing;
    }


    static void CheckDir(Player player)
    {
        var s = LeniencyHelperModule.Session;

        float saveDuration;

        if(ExtendBufferOnFreezeAndPickup.PickupTimer || player.minHoldTimer > 0f)
        {
            saveDuration = Math.Min(GetSetting<Time>(EarlyBackboostTiming),
                Player.HoldMinTime + ExtendBufferOnFreezeAndPickup.PickupTimer);
        }
        else
        {
            saveDuration = GetSetting<Time>(LateBackboostTiming);
        }

        if (player.moveX == 1) RightTimer.Launch(saveDuration);
        else if (player.moveX == -1) LeftTimer.Launch(saveDuration);
    }

    static void ConsumeBackboostFacing(On.Celeste.Player.orig_Throw orig, Player self)
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

        if(Math.Sign(self.Speed.X) == -1 && RightTimer)
        {
            savedPlayerFacing = self.Facing;
            self.Facing = Facings.Right;

            savedMoveY = Input.MoveY.Value;
            Input.MoveY.Value = 0;
        }
        else if(Math.Sign(self.Speed.X) == 1 && LeftTimer)
        {
            savedPlayerFacing = self.Facing;
            self.Facing = Facings.Left;

            savedMoveY = Input.MoveY.Value;
            Input.MoveY.Value = 0;
        }
        else
        {
            if (self.Facing == Facings.Left && RightTimer)
            {
                savedPlayerFacing = self.Facing;
                self.Facing = Facings.Right;

                savedMoveY = Input.MoveY.Value;
                Input.MoveY.Value = 0;
            }
            else if (self.Facing == Facings.Right && LeftTimer)
            {
                savedPlayerFacing = self.Facing;
                self.Facing = Facings.Left;

                savedMoveY = Input.MoveY.Value;
                Input.MoveY.Value = 0;
            }
        }

        orig(self);

        if (savedPlayerFacing.HasValue) self.Facing = savedPlayerFacing.Value;
        if (savedMoveY.HasValue) Input.MoveY.Value = savedMoveY.Value;
    }
}