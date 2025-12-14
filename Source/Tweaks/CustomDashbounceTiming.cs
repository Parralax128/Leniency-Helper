using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class CustomDashbounceTiming : AbstractTweak<CustomDashbounceTiming>
{
    static ILHook dashCoroutineHook;

    static Timer DashbounceTimer = new();
    [SaveState] static bool timerValid = false;
    [SaveState] static float dashDuration = 0.2f;
    [SaveState] static float varJumpTime = 0.25f;
    [SaveState] static bool? canDashbounce = false;

    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.SuperWallJump += SetCustomTiming;
        On.Celeste.Player.DashEnd += ConsumeDashbounce;
        On.Celeste.Player.DashBegin += TimerCheck;
        
        dashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Instance).GetStateMachineTarget(), GetDashDuration);
    }
    [OnUnload]
    public static void UnloadHooks() 
    {
        On.Celeste.Player.SuperWallJump -= SetCustomTiming;
        On.Celeste.Player.DashEnd -= ConsumeDashbounce;
        On.Celeste.Player.DashBegin -= TimerCheck;

        dashCoroutineHook.Dispose();
    }

    static void SetCustomTiming(On.Celeste.Player.orig_SuperWallJump orig, Player self, int dir)
    {
        orig(self, dir);
        varJumpTime = self.varJumpTimer;
        

        if (Enabled)
        {
            DashbounceTimer.Launch(GetSetting<Time>());
            timerValid = true;
        }
    }
    static void TimerCheck(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);


        if (!timerValid) return;

        canDashbounce = DashbounceTimer;
        timerValid = false;
        DashbounceTimer.Abort();

        if (Enabled && canDashbounce == false)
        {
            self.varJumpTimer = 0f;
        }
    }
    static void ConsumeDashbounce(On.Celeste.Player.orig_DashEnd orig, Player self)
    {
        if (canDashbounce == true)
        {   
            self.varJumpTimer = Math.Max(self.varJumpTimer, varJumpTime - (dashDuration + 0.05f + Engine.DeltaTime * 0.9f));
        }

        canDashbounce = null;

        orig(self);
    }

    static void GetDashDuration(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        while(cursor.TryGotoNext(MoveType.Before, i => i.MatchBox<float>()))
        {
            cursor.EmitDup();
            cursor.EmitDelegate(SetSessionDashDuration);
            cursor.GotoNext(MoveType.After, i => i.MatchBox<float>());
        }

        static void SetSessionDashDuration(float duration) => dashDuration = duration;
    }
}