using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class AutoSlowfall : AbstractTweak<AutoSlowfall>
{
    protected const int TechOnly = 0;
    protected const int DelayedJumpRelease = 1;
    protected const int ReleaseDelay = 2;

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


    private readonly static Timer ReleaseTimer = GetSetting<Time>(ReleaseDelay);
    private static bool TechState
    {
        get => GetTemp<bool>("TechState");
        set => SetTemp("TechState", value);
    }

    private static void CheckTechSpeed(Player player) {
        if (TechState && player.Speed.Y > 0f) TechState = false;
    }

    private static void LaunchTimer(ILContext il) => new ILCursor(il).EmitDelegate(StartTimer);
    private static void StartTimer() => ReleaseTimer.Launch(GetSetting<Time>(ReleaseDelay));
    private static void TechStateBegin(ILContext il) => new ILCursor(il).EmitDelegate(ToTechState);

    private static void ToTechState()
    {
        TechState = true;
        ReleaseTimer.Launch(GetSetting<Time>(ReleaseDelay));
    }


    private static int AutoSlowfallOnUpdate(On.Celeste.Player.orig_NormalUpdate orig, Player self)
    {
        if (Enabled)
        {
            if (GetSetting<bool>(DelayedJumpRelease)) self.AutoJump = ReleaseTimer & GetSetting<bool>(TechOnly) ? TechState : true;
            else self.AutoJump = GetSetting<bool>(TechOnly) ? TechState : true;
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