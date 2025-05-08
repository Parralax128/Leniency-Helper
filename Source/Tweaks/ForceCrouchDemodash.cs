namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class ForceCrouchDemodash : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.DashBegin += ForceCrouchDemodashDashBegin;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.DashBegin -= ForceCrouchDemodashDashBegin;
    }

    public static void ForceCrouchDemodashDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);
        if (Enabled("ForceCrouchDemodash") && self.demoDashed) self.Ducking = true;
    }
}