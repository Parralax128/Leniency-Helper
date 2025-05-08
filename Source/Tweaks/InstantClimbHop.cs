using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class InstantClimbHop : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Update += InstantClimbhopUpdate;
        On.Celeste.Player.ClimbHop += RedoMoveDown;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Update -= InstantClimbhopUpdate;
        On.Celeste.Player.ClimbHop -= RedoMoveDown;
    }
    public static void RedoMoveDown(On.Celeste.Player.orig_ClimbHop orig, Player self)
    {
        orig(self);

        if (Enabled("InstantClimbHop"))
        {
            LeniencyHelperModule.Session.savedClimbHopSolid = self.climbHopSolid;
            LeniencyHelperModule.Session.movedDown = false;
        }
    }
    public static void InstantClimbhopUpdate(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);

        if(!Enabled("InstantClimbHop")) return;

        var s = LeniencyHelperModule.Session;

        if(self.hopWaitX != 0f && s.savedClimbHopSolid != null)
        {
            if(!self.CollideCheck<Solid>(self.Position + Vector2.UnitX * (float)self.Facing))
                self.Speed.Y = 0f;

            if (self.CollideCheck<Solid>(self.Position + Vector2.UnitY * 
                (s.savedClimbHopSolid is null? 1f : s.savedClimbHopSolid.Speed.Y)
                * 4) && !s.movedDown)
            {
                self.MoveVExact((int)(self.Position.Y + 12 - 
                    (s.savedClimbHopSolid is null? self.Position.Y : s.savedClimbHopSolid.Position.Y)));
                s.movedDown = true;
            }
        }
    }
}