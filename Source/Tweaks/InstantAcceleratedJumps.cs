using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class InstantAcceleratedJumps
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Jump += GetFullWalkSpeed;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Jump -= GetFullWalkSpeed;
    }
    private static void GetFullWalkSpeed(On.Celeste.Player.orig_Jump orig, Player self,
        bool particles, bool playSfx)
    {
        if(!self.onGround || !LeniencyHelperModule.Session.Tweaks["InstantAcceleratedJumps"].Enabled)
        {
            orig(self, particles, playSfx);
            return; 
        }

        if(Math.Abs(self.Speed.X) < 90f && self.moveX != 0)
        {
            self.Speed.X = self.moveX * 90f;
        }
        orig(self, particles, playSfx);
    }
}