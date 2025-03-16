namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class DisableBackboost
{
    public static void LoadHooks()
    {
        On.Celeste.Player.Throw += ReturnSpeed;
    }
    public static void UnloadHooks() 
    {
        On.Celeste.Player.Throw -= ReturnSpeed;
    }

    private static void ReturnSpeed(On.Celeste.Player.orig_Throw orig, Player self)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["DisableBackboost"])
        {
            orig(self);
            return;
        }

        float savedSpeed = self.Speed.X;
        orig(self);
        self.Speed.X = savedSpeed;
    }

}