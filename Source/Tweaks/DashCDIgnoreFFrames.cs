using Monocle;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class DashCDIgnoreFFrames : AbstractTweak<DashCDIgnoreFFrames>
{
    [SaveState] static Player targetPlayer;
    [SaveState] static bool useOrigFreeze = false;

    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Celeste.Freeze += UpdateDashCDOnFreeze;
        On.Celeste.Player.Added += GetPlayerOnAdded;
        On.Celeste.Player.DashBegin += UseDefaultFFOnDash;
        On.Celeste.Player.DashUpdate += InstantDashBegin;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Celeste.Freeze -= UpdateDashCDOnFreeze;
        On.Celeste.Player.Added -= GetPlayerOnAdded;
        On.Celeste.Player.DashBegin -= UseDefaultFFOnDash;
        On.Celeste.Player.DashUpdate -= InstantDashBegin;
    }
    

    public static void UseDefaultFFOnDash(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        useOrigFreeze = true;
        orig(self);
        useOrigFreeze = false;
    }   
    public static void GetPlayerOnAdded(On.Celeste.Player.orig_Added orig, Player self, Scene scene)
    {
        orig(self, scene);
        targetPlayer = self;
    }
    public static void UpdateDashCDOnFreeze(On.Celeste.Celeste.orig_Freeze orig, float time)
    {
        orig(time);
        if(!Enabled || useOrigFreeze) return;
        {
            if (targetPlayer != null)
                targetPlayer.dashCooldownTimer -= time;
        }
    }

    public static int InstantDashBegin(On.Celeste.Player.orig_DashUpdate orig, Player self)
    {
        if (!Enabled) return orig(self);

        if (self.CanDash)
        {
            self.StartDash();
            self.StateMachine.ForceState(Player.StDash);
            return Player.StDash;
        }

        return orig(self);
    }
}