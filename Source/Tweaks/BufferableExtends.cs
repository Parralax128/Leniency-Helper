using Monocle;
using System;
using System.Linq;
using MonoMod.Cil;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class BufferableExtends : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.DashUpdate += AddSuperjumpCheckOnUpdate;
        On.Celeste.Player.Update += CheckForDashStart;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.DashUpdate -= AddSuperjumpCheckOnUpdate;
        On.Celeste.Player.Update -= CheckForDashStart;
    }

    public static bool CanSuperjump(Player player)
    {
        if (player.Dashes >= player.MaxDashes || !Enabled("BufferableExtends")) return true;
        if (GetSetting<bool>("forceWaitForRefill")) return false;

        if (Enabled("RefillDashInCoyote"))
        {
            float refillTimer = player.Get<Components.RefillCoyoteComponent>().refillCoyoteTimer;

            if (refillTimer > player.dashRefillCooldownTimer
                && GetSetting<float>("extendsTiming") > player.dashRefillCooldownTimer
                && Input.Jump.bufferCounter > player.dashRefillCooldownTimer)
            {
                return false;
            }
        }

        int saveDashes = player.Dashes;
        if (player.dashRefillCooldownTimer <= 0f && !player.RefillDash())
        {
            player.Dashes = saveDashes;
            return true;
        }
        player.Dashes = saveDashes;

        if (!player.Inventory.NoRefills
            && GetSetting<float>("extendsTiming") - Engine.DeltaTime > player.dashRefillCooldownTimer
            && Input.Jump.bufferCounter - Engine.DeltaTime > player.dashRefillCooldownTimer
            && Module.LeniencyHelperModule.Session.dashTimer <= 0f)
        {
            return false;
        }

        return true;
    }

    private static void CheckForDashStart(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);
        if (!Enabled("BufferableExtends")) return;

        var s = Module.LeniencyHelperModule.Session;
        if (s.dashTimer > 0f) s.dashTimer -= Engine.DeltaTime;

        if (self.StateMachine.State != s.prevFrameState && (new int[] { 2, 4, 5 }).Contains(self.StateMachine.State))
        {
            s.dashTimer = 2 * Engine.DeltaTime;
        }
        s.prevFrameState = self.StateMachine.State;
    }
    private static void AddSuperjumpCheckOnUpdate(ILContext il)
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