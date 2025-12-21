using Monocle;
using System;
using System.Linq;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class BufferableExtends : AbstractTweak<BufferableExtends>
{
    [SettingIndex] static int ForceWaitForRefill;
    [SettingIndex] static int ExtendTiming;

    static Timer DashTimer = new();

    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.DashUpdate += AddSuperjumpCheckOnUpdate;
        Everest.Events.Player.OnAfterUpdate += CheckForDashStart;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.DashUpdate -= AddSuperjumpCheckOnUpdate;
        Everest.Events.Player.OnAfterUpdate -= CheckForDashStart;
    }

    public static bool CanSuperjump(Player player)
    {
        if (!Enabled || player.Dashes >= player.MaxDashes) return true;
        if (GetSetting<bool>(ForceWaitForRefill)) return false;


        int savedDashes = player.Dashes;
        if (player.dashRefillCooldownTimer <= 0f && !player.RefillDash())
        {
            player.Dashes = savedDashes;
            return true; // allowing to jump without dash refilled in case player cannot do that
        }
        player.Dashes = savedDashes;


        // checking: core mode, jump timing being in a time range of the buffer window, instant tech (first 2 frames of the dash)
        if (!player.Inventory.NoRefills
            && Math.Min(GetSetting<Time>(ExtendTiming), Input.Jump.bufferCounter) - Engine.DeltaTime > player.dashRefillCooldownTimer
            && DashTimer.Expired)
        {
            return false;
        }

        return true;
    }

    static void CheckForDashStart(Player player)
    {
        if (!Enabled) return;
        
        if (player.StateMachine.State != ExtendBufferOnFreezeAndPickup.prevFrameState && (new int[] { 2, 4, 5 }).Contains(player.StateMachine.State))
        {
            DashTimer.Launch(1.9f * Engine.DeltaTime); // set timer for 2f to check for instant tech
        }
        ExtendBufferOnFreezeAndPickup.prevFrameState = player.StateMachine.State;
    }
    static void AddSuperjumpCheckOnUpdate(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        ILLabel label = null;
            
        if (cursor.TryGotoNext(MoveType.Before,
            instr => instr.MatchBleUn(out label),
            instr => instr.MatchLdarg0(),
            instr => instr.MatchCallvirt<Player>("SuperJump")))
        {
            cursor.Index++;
            cursor.EmitLdarg0();
            cursor.EmitDelegate(CanSuperjump);
            cursor.EmitBrfalse(label);
        }
    }
}