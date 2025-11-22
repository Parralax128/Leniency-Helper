using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class AutoSlowfall : AbstractTweak<AutoSlowfall>
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.NormalUpdate += AutoSlowfallOnUpdate;
        IL.Celeste.Player.SuperJump += TechStateBegin;
        IL.Celeste.Player.SuperWallJump += TechStateBegin;
        Everest.Events.Player.OnBeforeUpdate += CheckTechSpeed;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.NormalUpdate -= AutoSlowfallOnUpdate;
        IL.Celeste.Player.SuperJump -= TechStateBegin;
        IL.Celeste.Player.SuperWallJump -= TechStateBegin;
        Everest.Events.Player.OnBeforeUpdate -= CheckTechSpeed;
    }

    private static bool ManualSlowfall = false;
    private static void CheckTechSpeed(Player player)
    {
        var s = Module.LeniencyHelperModule.Session;
        if (s.inTechState)
        {
            if (player.Speed.Y > 0f) s.inTechState = false;
            if (s.jumpReleaseTimer > 0f) s.jumpReleaseTimer -= Engine.DeltaTime;
            else s.inTechState = false;
        }
    }

    private static void TechStateBegin(ILContext il)
    {
        (new ILCursor(il)).EmitDelegate(ToTechState);
    }
    private static void ToTechState()
    {
        Module.LeniencyHelperModule.Session.inTechState = true;
        Module.LeniencyHelperModule.Session.jumpReleaseTimer = GetTime("releaseDelay");
    }
    private static bool Slowfall
    {
        get 
        {
            if (GetSetting<bool>("delayedJumpRelease"))
            {
                if (Module.LeniencyHelperModule.Session.jumpReleaseTimer <= 0f)
                    return false;
            }
            return GetSetting<bool>("techOnly") ? Module.LeniencyHelperModule.Session.inTechState : true;
        }
    }
    private static int AutoSlowfallOnUpdate(On.Celeste.Player.orig_NormalUpdate orig, Player self)
    {
        ManualSlowfall = Slowfall;
        if (Enabled) self.AutoJump = (self.AutoJump && !ManualSlowfall) || ManualSlowfall;
        Log(ManualSlowfall);
        int newState = orig(self);
        if(newState != 0)
        {
            ManualSlowfall = false;
            Module.LeniencyHelperModule.Session.jumpReleaseTimer = 0f;
            Module.LeniencyHelperModule.Session.inTechState = false;
        }
        return newState;
    }
}   