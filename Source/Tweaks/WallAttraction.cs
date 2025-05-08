using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Monocle;
using static Celeste.Mod.LeniencyHelper.SettingMaster;
using Celeste.Mod.MaxHelpingHand.Entities;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class WallAttraction : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Update += AttractUpdate;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Update -= AttractUpdate;
    }

    private static int[] noGrabStates = { 2, 4, 5, 7, 10, 11, 16, 17, 18, 20, 21, 22, 23, 24, 25 };

    private static void AttractUpdate(On.Celeste.Player.orig_Update orig, Player self)
    {
        if (!Enabled("WallAttraction"))
        {
            orig(self);
            return;
        }

        if (Input.GrabCheck && !self.IsTired && !noGrabStates.Contains(self.StateMachine.State) && self.CanUnDuck &&
            Math.Sign(self.Speed.X) != -(int)self.Facing && self.Holding == null && self.Speed.Y >= 0)
        {
            float distance = GetSetting<float>("wallApproachTime") * (Math.Abs(self.Speed.X) /
                (GetSetting<bool>("countAttractionTimeInFrames") ? Engine.FPS : 1f));

            Vector2 origPos = self.Position;
            for (int c = 0; c < (int)distance; c++)
            {
                self.Position.X += c * (int)self.Facing;
                Vector2 solidCheckPos = self.Position + Vector2.UnitX * (int)self.Facing;

                if (ClimbBlocker.Check(self.Scene, self, self.Position + Vector2.UnitX * 2f * (float)self.Facing))
                {
                    self.Position = origPos;
                    break;
                }
                else if (self.ClimbBoundsCheck((int)self.Facing) && self.CollideCheck<Solid>(solidCheckPos) 
                    || (LeniencyHelperModule.ModLoaded("MaxHelpingHand") && CollidingWithSideways(self, solidCheckPos)))
                {
                    break;
                }
                else self.Position = origPos;
            }
        }

        orig(self);
    }


    private static bool CollidingWithSideways(Player player, Vector2 at)
    {
        SidewaysJumpThru jt = player.CollideFirstOutside<SidewaysJumpThru>(at);
        return jt != null && (jt.AllowLeftToRight == (player.Position.X > at.X))
            && jt.Bottom >= player.Top + at.Y - player.Position.Y + 3;
    }
}