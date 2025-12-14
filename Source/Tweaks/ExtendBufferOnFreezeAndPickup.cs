using Monocle;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using MonoMod.Utils;
using System.Reflection;
namespace Celeste.Mod.LeniencyHelper.Tweaks;

class ExtendBufferOnFreezeAndPickup : AbstractTweak<ExtendBufferOnFreezeAndPickup>
{
    [SettingIndex] static int OnFreeze;
    [SettingIndex] static int OnPickup;


    [SaveState] static float pickupDelay = 0.016f;
    [SaveState] public static int prevFrameState = Player.StNormal;

    [SaveState] static bool jumpExtended = false;
    [SaveState] static bool dashExtended = false;
    [SaveState] static bool demoExtended = false;

    public static Timer PickupTimer = new();


    static ILHook customPickupDelayHook;

    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Celeste.Freeze += ExtendOnFreeze;
        Everest.Events.Player.OnBeforeUpdate += ExtendBufferOnPickup;

        customPickupDelayHook = new ILHook(typeof(Player).GetMethod("PickupCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), GetCustomPickupDelayHook);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Celeste.Freeze -= ExtendOnFreeze;
        Everest.Events.Player.OnBeforeUpdate -= ExtendBufferOnPickup;
        customPickupDelayHook.Dispose();
    }

    static void ExtendBufferOnPickup(Player player)
    {        
        if (player.StateMachine.State == Player.StPickup)
        {
            if (prevFrameState != Player.StPickup) PickupTimer.Launch(pickupDelay);

            if(Enabled && PickupTimer && GetSetting<bool>(OnPickup))
            {
                if (Input.Dash.Pressed && !dashExtended)
                {
                    Input.Dash.bufferCounter += PickupTimer;
                    dashExtended = true;
                }
                if (Input.CrouchDash.Pressed && !demoExtended)
                {
                    Input.CrouchDash.bufferCounter += PickupTimer;
                    demoExtended = true;
                }
                if (Input.Jump.Pressed && !jumpExtended)
                {
                    Input.Jump.bufferCounter += PickupTimer;
                    jumpExtended = true;
                }
            }
        }
        else dashExtended = demoExtended = jumpExtended = false;
        

        prevFrameState = player.StateMachine.State;
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

        static void GetNewCustomPickupDelay(float delay) => pickupDelay = delay;
    }
    
    
    public static void ExtendOnFreeze(On.Celeste.Celeste.orig_Freeze orig, float time)
    {
        orig(time);
        
        if (!Enabled || !GetSetting<bool>(OnFreeze)) return;

        if(Input.Dash.bufferCounter > 0f) Input.Dash.bufferCounter += time;
        if(Input.CrouchDash.bufferCounter > 0f) Input.CrouchDash.bufferCounter += time;
        if(Input.Jump.bufferCounter > 0f)  Input.Jump.bufferCounter += time;
    }
}