using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;
class AutoSlowfall : AbstractTweak<AutoSlowfall>
{
    [SettingIndex] static int TechOnly;
    [SettingIndex] static int DelayedJumpRelease;
    [SettingIndex] static int ReleaseDelay;
    
    static Timer ReleaseTimer = new();
    [SaveState] static bool TechState;


    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.NormalUpdate += AutoSlowfallOnUpdate;
        IL.Celeste.Player.Jump += LaunchTimer;
        IL.Celeste.Player.SuperJump += TechStateBegin;
        IL.Celeste.Player.SuperWallJump += TechStateBegin;
        Everest.Events.Player.OnBeforeUpdate += CheckTechSpeed;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.NormalUpdate -= AutoSlowfallOnUpdate;
        IL.Celeste.Player.Jump -= LaunchTimer;
        IL.Celeste.Player.SuperJump -= TechStateBegin;
        IL.Celeste.Player.SuperWallJump -= TechStateBegin;
        Everest.Events.Player.OnBeforeUpdate -= CheckTechSpeed;
    }

    static void CheckTechSpeed(Player player) 
    {
        if (TechState && player.Speed.Y > 0f) TechState = false;
    }

    static void LaunchTimer(ILContext il)
    {
        new ILCursor(il).EmitDelegate(StartTimer);
        static void StartTimer() => ReleaseTimer.Launch(GetSetting<Time>(ReleaseDelay));
    }
    
    static void TechStateBegin(ILContext il) => new ILCursor(il).EmitDelegate(ToTechState);

    static void ToTechState()
    {
        TechState = true;
        ReleaseTimer.Launch(GetSetting<Time>(ReleaseDelay));
    }


    static int AutoSlowfallOnUpdate(On.Celeste.Player.orig_NormalUpdate orig, Player self)
    {
        if (Enabled)
        {
            self.AutoJump = (GetSetting<bool>(DelayedJumpRelease) ? ReleaseTimer : true)
                && (GetSetting<bool>(TechOnly) ? TechState : true);
        }
        
        int newState = orig(self);
        if(newState != Player.StNormal)
        {
            ReleaseTimer.Abort();
            TechState = false;
        }
        return newState;
    }
}