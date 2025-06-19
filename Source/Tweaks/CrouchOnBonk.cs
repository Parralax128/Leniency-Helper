namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CrouchOnBonk : AbstractTweak
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
        if(Enabled("CrouchOnBonk") && !self.Ducking && (self.PreviousPosition.Y > self.Position.Y || self.Speed.Y <= 0f))
        {
            self.Ducking = true;
        }
        else orig(self, data);
    }
}