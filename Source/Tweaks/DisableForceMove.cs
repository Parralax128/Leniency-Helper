namespace Celeste.Mod.LeniencyHelper.Tweaks;
public class DisableForceMove
{
    public static void LoadHooks()
    {
        On.Celeste.Player.Update += ClearForcemoveOnUpdate;
    }
    public static void UnloadHooks()
    {
        On.Celeste.Player.Update -= ClearForcemoveOnUpdate;
    }

    private static void ClearForcemoveOnUpdate(On.Celeste.Player.orig_Update orig, Player self)
    {
        if (LeniencyHelperModule.Session.TweaksEnabled["DisableForceMove"])
        {
            if(self.forceMoveXTimer != 0f) self.forceMoveXTimer = 0f;
            if(self.forceMoveX != 0) self.forceMoveX = 0;
        }

        orig(self);
    }
}