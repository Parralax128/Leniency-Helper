using Monocle;
using System;
using System.Linq;
using MonoMod.Cil;
using Celeste.Mod.LeniencyHelper.Module;

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
        if (player.Dashes >= player.MaxDashes || !Enabled) return true;
        if (GetSetting<bool>(ForceWaitForRefill)) return false;


        if (RefillDashInCoyote.Enabled
            && player.Get<Components.RefillCoyoteComponent>().RefillCoyoteTimer > player.dashRefillCooldownTimer
            && GetSetting<Time>(ExtendTiming) > player.dashRefillCooldownTimer
            && Input.Jump.bufferCounter > player.dashRefillCooldownTimer) 
        {
                return false;
        }

        int savedDashes = player.Dashes;
        if (player.dashRefillCooldownTimer <= 0f && !player.RefillDash())
        {
            player.Dashes = savedDashes;
            return true;
        }
        player.Dashes = savedDashes;

        // checks for: core mode, jump timing being in a time range of the buffer window, instant tech (first 2 frames of the dash)
        if (!player.Inventory.NoRefills
            && Math.Min(GetSetting<Time>(ExtendTiming), Input.Jump.bufferCounter) - Engine.DeltaTime > player.dashRefillCooldownTimer
            && DashTimer) 
        {
            return false;
        }

        return true;
    }

    static void CheckForDashStart(Player player)
    {
        if (!Enabled) return;

        var s = LeniencyHelperModule.Session;

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