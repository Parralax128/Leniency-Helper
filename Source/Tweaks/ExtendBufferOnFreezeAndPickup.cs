using Monocle;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using MonoMod.Utils;
using System.Reflection;
using Celeste.Mod.LeniencyHelper.Module;
namespace Celeste.Mod.LeniencyHelper.Tweaks;

class ExtendBufferOnFreezeAndPickup : AbstractTweak<ExtendBufferOnFreezeAndPickup>
{
    [SettingIndex] static int OnFreeze;
    [SettingIndex] static int OnPickup;

    static ILHook customPickupDelayHook;

    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Celeste.Freeze += ExtendBufferTimer;
        Everest.Events.Player.OnBeforeUpdate += ExtendBufferOnPickup;

        customPickupDelayHook = new ILHook(typeof(Player).GetMethod("PickupCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), GetCustomPickupDelayHook);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Celeste.Freeze -= ExtendBufferTimer;
        Everest.Events.Player.OnBeforeUpdate -= ExtendBufferOnPickup;
        customPickupDelayHook.Dispose();
    }

    public static Timer PickupTimer = new();

    static void ExtendBufferOnPickup(Player player)
    {
        var s = LeniencyHelperModule.Session;
        
        if (player.StateMachine.State == 8)
        {
            if (s.prevFrameState != 8) PickupTimer.Set(s.pickupDelay);

            if(PickupTimer && Enabled && GetSetting<bool>(OnPickup))
            {
                if (Input.Dash.Pressed && !s.dashExtended)
                {
                    Input.Dash.bufferCounter += PickupTimer;
                    s.dashExtended = true;
                }
                if (Input.CrouchDash.Pressed && !s.demoExtended)
                {
                    Input.CrouchDash.bufferCounter += PickupTimer;
                    s.demoExtended = true;
                }
                if (Input.Jump.Pressed && !s.jumpExtended)
                {
                    Input.Jump.bufferCounter += PickupTimer;
                    s.jumpExtended = true;
                }
            }
        }
        else
        {
            s.dashExtended = s.demoExtended = s.jumpExtended = false;
        }
        s.prevFrameState = player.StateMachine.State;
    }
    static void GetCustomPickupDelayHook(ILContext il)
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
    
    static void GetNewCustomPickupDelay(float delay)
    {
        LeniencyHelperModule.Session.pickupDelay = delay;
    }
    public static void ExtendBufferTimer(On.Celeste.Celeste.orig_Freeze orig, float time)
    {
        orig(time);
        
        if (!(Enabled && GetSetting<bool>(OnFreeze))) return;

        if(Input.Dash.bufferCounter > 0f) Input.Dash.bufferCounter += time;
        if(Input.CrouchDash.bufferCounter > 0f) Input.CrouchDash.bufferCounter += time;
        if(Input.Jump.bufferCounter > 0f)  Input.Jump.bufferCounter += time;
    }
}
