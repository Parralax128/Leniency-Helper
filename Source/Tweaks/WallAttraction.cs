using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Celeste.Mod.MaxHelpingHand.Entities;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class WallAttraction : AbstractTweak<WallAttraction>
{
    [SettingIndex("Mode")] static int Distance;


    [OnLoad]
    public static void LoadHooks()
    {
        Everest.Events.Player.OnBeforeUpdate += AttractUpdate;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        Everest.Events.Player.OnBeforeUpdate -= AttractUpdate;
    }

    static readonly int[] noGrabStates = { 2, 4, 5, 7, 10, 11, 16, 17, 18, 20, 21, 22, 23, 24, 25 };

    static void AttractUpdate(Player player)
    {
        if (!Enabled) return;
       

        if (Input.GrabCheck && !player.IsTired && !noGrabStates.Contains(player.StateMachine.State) && player.CanUnDuck &&
            Math.Sign(player.Speed.X) != -(int)player.Facing && player.Holding == null && player.Speed.Y >= 0)
        {
            float distance = GetFlexDistance(Distance, Math.Abs(player.Speed.X));

            Vector2 origPos = player.Position;
            for (int c = 0; c < (int)distance; c++)
            {
                player.Position.X += c * (int)player.Facing;
                Vector2 solidCheckPos = player.Position + Vector2.UnitX * (int)player.Facing;

                if (ClimbBlocker.Check(player.Scene, player, player.Position + Vector2.UnitX * 2f * (float)player.Facing))
                {
                    player.Position = origPos;
                    break;
                }
                else if (player.ClimbBoundsCheck((int)player.Facing) && player.CollideCheck<Solid>(solidCheckPos) 
                    || LeniencyHelperModule.ModLoaded("MaxHelpingHand") && CollidingWithSidewaysJT(player, solidCheckPos))
                {
                    break;
                }
                else player.Position = origPos;
            }
        }
    }

    static bool CollidingWithSidewaysJT(Player player, Vector2 at)
    {
        SidewaysJumpThru jt = player.CollideFirstOutside<SidewaysJumpThru>(at);
        return jt != null && jt.AllowLeftToRight == player.Position.X > at.X
            && jt.Bottom >= player.Top + at.Y - player.Position.Y + 3;
    }
}