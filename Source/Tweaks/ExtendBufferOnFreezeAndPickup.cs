using Monocle;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using MonoMod.Utils;
using System.Reflection;
namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class ExtendBufferOnFreezeAndPickup
{
    private static ILHook customPickupDelayHook;
    public static void LoadHooks()
    {
        On.Celeste.Celeste.Freeze += ExtendBufferTimer;
        On.Celeste.Player.Update += ExtendBufferOnPickup;
        customPickupDelayHook = new ILHook(typeof(Player).GetMethod("PickupCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), GetCustomPickupDelayHook);
    }
    public static void UnloadHooks()
    {
        On.Celeste.Celeste.Freeze -= ExtendBufferTimer;
        On.Celeste.Player.Update -= ExtendBufferOnPickup;
        customPickupDelayHook.Dispose();
    }
    private static void ExtendBufferOnPickup(On.Celeste.Player.orig_Update orig, Player self)
    {
        var s = LeniencyHelperModule.Session;
        if (!(LeniencyHelperModule.Session.TweaksEnabled["ExtendBufferOnFreezeAndPickup"] && 
            (bool)LeniencyHelperModule.Settings.GetSetting("ExtendBufferOnFreezeAndPickup", "ExtendBufferOnPickup")))
        {
            orig(self);
            return;
        }
        if (s.pickupTimeLeft > 0f)
            s.pickupTimeLeft -= Engine.DeltaTime;
        
        if (self.StateMachine.State == 8)
        {
            if (s.prevFrameState != 8)
                s.pickupTimeLeft = s.pickupDelay;

            if(s.pickupTimeLeft > 0f)
            {
                if (Input.Dash.Pressed && !s.dashExtended)
                {
                    Input.Dash.bufferCounter += s.pickupTimeLeft;
                    s.dashExtended = true;
                }
                if (Input.CrouchDash.Pressed && !s.demoExtended)
                {
                    Input.CrouchDash.bufferCounter += s.pickupTimeLeft;
                    s.demoExtended = true;
                }
                if (Input.Jump.Pressed && !s.jumpExtended)
                {
                    Input.Jump.bufferCounter += s.pickupTimeLeft;
                    s.jumpExtended = true;
                }
            }
        }
        else
        {
            s.dashExtended = s.demoExtended = s.jumpExtended = false;
        }
        s.prevFrameState = self.StateMachine.State;
        orig(self);
    }
    private static void GetCustomPickupDelayHook(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        if (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCall<Tween>("Create")))
        {
            if (cursor.TryGotoPrev(MoveType.Before, instr => instr.MatchLdcI4(1)))
            {
                cursor.EmitDup();
                cursor.EmitDelegate(GetNewCustomPickupDelay);
            }
        }
    }
    private static void GetNewCustomPickupDelay(float delay)
    {
        LeniencyHelperModule.Session.pickupDelay = delay;
    }
    public static void ExtendBufferTimer(On.Celeste.Celeste.orig_Freeze orig, float time)
    {
        orig(time);
        
        if (!(LeniencyHelperModule.Session.TweaksEnabled["ExtendBufferOnFreezeAndPickup"]
            && (bool)LeniencyHelperModule.Settings.GetSetting("ExtendBufferOnFreezeAndPickup", "ExtendBufferOnFreeze"))) return;

        if(Input.Dash.bufferCounter > 0f) Input.Dash.bufferCounter += time;
        if(Input.CrouchDash.bufferCounter > 0f) Input.CrouchDash.bufferCounter += time;
        if(Input.Jump.bufferCounter > 0f)  Input.Jump.bufferCounter += time;
    }
}

