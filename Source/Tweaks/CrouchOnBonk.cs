using Celeste.Mod.LeniencyHelper.CrossModSupport;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class CrouchOnBonk : AbstractTweak<CrouchOnBonk>
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.OnCollideV += CrouchOnCollide;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.OnCollideV -= CrouchOnCollide;
    }

    public static void CrouchOnCollide(On.Celeste.Player.orig_OnCollideV orig, Player self, CollisionData data)
    {
        if (Enabled && !self.Ducking && Math.Sign(self.Speed.Y) == -GravityHelperImports.currentGravity)
        {
            self.Ducking = true;
        }
        else orig(self, data);
    }
}