using Celeste.Mod.LeniencyHelper.Module;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CustomDashbounceTiming : AbstractTweak<CustomDashbounceTiming>
{
    private static ILHook dashCoroutineHook;

    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.SuperWallJump += SetCustomTiming;
        On.Celeste.Player.DashEnd += ConsumeDashbounce;
        LeniencyHelperModule.OnUpdate += UpdateTimer;
        On.Celeste.Player.DashBegin += TimerCheck;
        
        dashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Instance).GetStateMachineTarget(), GetDashDuration);
    }
    [OnUnload]
    public static void UnloadHooks() 
    {
        On.Celeste.Player.SuperWallJump -= SetCustomTiming;
        On.Celeste.Player.DashEnd -= ConsumeDashbounce;
        LeniencyHelperModule.OnUpdate -= UpdateTimer;
        On.Celeste.Player.DashBegin -= TimerCheck;

        dashCoroutineHook.Dispose();
    }

    private static void SetCustomTiming(On.Celeste.Player.orig_SuperWallJump orig, Player self, int dir)
    {
        orig(self, dir);
        LeniencyHelperModule.Session.varJumpTime = self.varJumpTimer;
        

        if (Enabled)
        {
            LeniencyHelperModule.Session.dashbounceTimer = GetTime("dashbounceTiming");
        }
    }
    private static void UpdateTimer()
    {
        if (LeniencyHelperModule.Session.dashbounceTimer.HasValue
            && LeniencyHelperModule.Session.dashbounceTimer > 0f)
        {
            LeniencyHelperModule.Session.dashbounceTimer -= Engine.DeltaTime;
        }
    }
    private static void TimerCheck(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);

        var s = LeniencyHelperModule.Session;

        if (s.dashbounceTimer == null) return;

        s.canDashbounce = s.dashbounceTimer > 0f;
        s.dashbounceTimer = null;

        if (Enabled && s.canDashbounce == false)
        {
            self.varJumpTimer = 0f;
        }
    }
    private static void ConsumeDashbounce(On.Celeste.Player.orig_DashEnd orig, Player self)
    {
        var s = LeniencyHelperModule.Session;
        if (s.canDashbounce == true)
        {   
            self.varJumpTimer = Math.Max(self.varJumpTimer, s.varJumpTime - (s.dashDuration + 0.05f + Engine.DeltaTime * 0.9f));
        }

        s.canDashbounce = null;

        orig(self);
    }

    private static void GetDashDuration(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        while(cursor.TryGotoNext(MoveType.Before, i => i.MatchBox<float>()))
        {
            cursor.EmitDup();
            cursor.EmitDelegate(SetSessionDashDuration);
            cursor.GotoNext(MoveType.After, i => i.MatchBox<float>());
        }
    }
    private static void SetSessionDashDuration(float duration) => 
        LeniencyHelperModule.Session.dashDuration = duration;
}