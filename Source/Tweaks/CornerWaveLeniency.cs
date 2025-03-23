using Monocle;
using Microsoft.Xna.Framework;  
using System;
using static Celeste.Mod.LeniencyHelper.CrossModSupport.GravityHelperImports;
using System.Linq;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CornerWaveLeniency
{
    public static void LoadHooks()
    {
        On.Celeste.Player.WallJump += WJIntoWave;
        IL.Celeste.Player.OnCollideV += RemoveDiagCCorection;
    }
    public static void UnloadHooks()
    {
        On.Celeste.Player.WallJump -= WJIntoWave;
        IL.Celeste.Player.OnCollideV -= RemoveDiagCCorection;
    }
    private static bool CheckDiag(Player player)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["CornerWaveLeniency"]) return false;

        if ((new int[] { 2, 4, 5 }).Contains(player.StateMachine.State))
        {
            return player.DashDir.X != 0f && player.DashDir.Y > 0f && player.Speed.Y > 0f;
        }
        else
        {
            return Math.Sign(player.Speed.Y) == currentGravity && player.Speed.Y > 0f;
        }
    }
    private static void RemoveDiagCCorection(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        if (cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdcI4(1),
            instr => instr.MatchCall<Actor>("OnGround")))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(CheckDiag);

            cursor.EmitOr();
        }

        if (cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdcI4(1),
            instr => instr.MatchCall<Actor>("OnGround")))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(CheckDiag);

            cursor.EmitOr();
        }
    }
    private static void WJIntoWave(On.Celeste.Player.orig_WallJump orig, Player self, int dir)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["CornerWaveLeniency"])
        {
            orig(self, dir);
            return;
        }
        if (Math.Sign(self.Speed.X) == dir && CheckCorner(self, dir))
        {
            var s = LeniencyHelperModule.Session;
            if ((new int[] { 2, 4, 5 }).Contains(self.StateMachine.State))
            {
                self.Ducking = (self.DashDir.Y > 0f && self.DashDir.X != 0f);
                if (self.StateMachine.State != 5) self.SuperJump();
                else self.Jump();
                DoCCorection(self);
                RefillDashIfCan(self);
            }
            else
            {
                self.Jump();
                DoCCorection(self);
                RefillDashIfCan(self);
            }
            return;
        }
        orig(self, dir);
    }
    private static CollisionData GetCollisionData(Vector2 pos, Player player)
    {
        CollisionData data;
        data.Direction = new Vector2(0f, currentGravity);

        Platform hitPlatform = player.CollideFirst<Platform>(pos);
        data.Hit = hitPlatform;
        data.Moved = data.Direction;

        Solid solidPusher = player.CollideFirst<Solid>(pos);
        if (solidPusher == null) data.Pusher = null;
        else data.Pusher = solidPusher.LiftSpeed == Vector2.Zero ? null : solidPusher;

        data.TargetPosition = new Vector2(player.Position.X + (player.Speed.X / 60f), pos.Y);

        return data;
    }
    private static bool CheckCorner(Player player, int dir)
    {
        Vector2 origPos = player.Position;
        Vector2 checkPos = origPos;

        int vertPosCheck = (int)Math.Abs(player.Speed.Y / 30);
        checkPos.X += (LeniencyHelperModule.Session.wjDist + 1) * -dir;
        checkPos.Y = currentGravity == 1? player.Bottom : player.Top;

        for (int c=0; c<vertPosCheck; c++)
        {
            if (player.OnGround(checkPos + Vector2.UnitY * -c * currentGravity))
            {
                if(!player.OnGround(checkPos + Vector2.UnitY * -(c+1) * currentGravity))
                {

                    if ((bool)LeniencyHelperModule.Settings.GetSetting("CornerWaveLeniency", "allowSpikedFloor"))
                    {
                        player.OnCollideV(GetCollisionData(checkPos + Vector2.UnitY * -c * currentGravity, player));
                        return true;
                    }

                    foreach (Entity entity in player.Scene.Entities)
                    {
                        if (player.CollideCheck(entity, checkPos))
                        {
                            bool hasSwitchComponent = entity.Get<Switch>() != null;

                            if (!player.CollideCheck(entity) && entity is not Trigger
                                && entity is not Solid && entity is not Actor && !hasSwitchComponent && 
                                entity.GetType().Name.ToLower().Contains("spike"))
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                
            }
        }
        return false;
    }
    private static void RefillDashIfCan(Player player)
    {
        if (player.dashRefillCooldownTimer <= 0f && !player.Inventory.NoRefills)
        {
            player.RefillDash();
        }
        player.RefillStamina();
    }
    private static void DoCCorection(Player player)
    {
        float possiblePosY = player.Position.Y;

        for (int i = 0; i <= 4; i++)
        {
            possiblePosY -= currentGravity;
            if (!player.CollideCheck<Solid>(new Vector2(player.Position.X + (player.Speed.X / Engine.FPS), possiblePosY)))
            {
                player.MoveVExact(-i);
                player.MoveHExact((int)(player.Speed.X * Engine.DeltaTime));
                break;
            }
        }
    }
}