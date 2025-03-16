namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class ForceCrouchDemodash
{
    public static void LoadHooks()
    {
        On.Celeste.Player.DashBegin += ForceCrouchDemodashDashBegin;
    }
    public static void UnloadHooks()
    {
        On.Celeste.Player.DashBegin -= ForceCrouchDemodashDashBegin;
    }

    public static void ForceCrouchDemodashDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);
        if (LeniencyHelperModule.Session.TweaksEnabled["ForceCrouchDemodash"] && self.demoDashed) self.Ducking = true;
    }
}