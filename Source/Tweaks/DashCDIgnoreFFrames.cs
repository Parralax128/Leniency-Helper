using Monocle;
using Microsoft.Xna.Framework;
using System;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class DashCDIgnoreFFrames : AbstractTweak
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
        if(!Enabled("DashCDIgnoreFFrames") || s.useOrigFreeze) return;
        {
            if (s.modifiedPlayer is not null)
                s.modifiedPlayer.dashCooldownTimer -= time;
        }
    }

    public static int InstantDashBegin(On.Celeste.Player.orig_DashUpdate orig, Player self)
    {
        if (!Enabled("DashCDIgnoreFFrames")) return orig(self);

        if (self.CanDash)
        {
            ForceEndDash(self);
            self.Speed += self.LiftSpeed;
            return self.StartDash();
        }

        return orig(self);
    }
    private static void ForceEndDash(Player player)
    {
        Vector2 swapCancel = Vector2.One;
        foreach (SwapBlock entity in player.Scene.Tracker.GetEntities<SwapBlock>())
        {
            if (player.CollideCheck(entity, player.Position + Vector2.UnitY) && entity != null && entity.Swapping)
            {
                if (player.DashDir.X != 0f && entity.Direction.X == (float)Math.Sign(player.DashDir.X))
                {
                    player.Speed.X = (swapCancel.X = 0f);
                }
                if (player.DashDir.Y != 0f && entity.Direction.Y == (float)Math.Sign(player.DashDir.Y))
                {
                    player.Speed.Y = (swapCancel.Y = 0f);
                }   
            }
        }
        player.CreateTrail();
        player.AutoJump = true;
        player.AutoJumpTimer = 0f;
        if (player.DashDir.Y <= 0f)
        {
            player.Speed = player.DashDir * 160f;
            player.Speed.X *= swapCancel.X;
            player.Speed.Y *= swapCancel.Y;
        }
        if (player.Speed.Y < 0f) player.Speed.Y *= 0.75f;

        player.StateMachine.State = Player.StNormal;
        player.DashEnd();
    }
}