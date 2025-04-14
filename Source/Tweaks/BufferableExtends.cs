using Monocle;
using System;
using System.Linq;
using MonoMod.Cil;
using Celeste.Mod.LeniencyHelper.CrossModSupport;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class BufferableExtends
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
        if (player.Dashes >= player.MaxDashes || !LeniencyHelperModule.Session.Tweaks["BufferableExtends"].Enabled) return true;

        float? refillTimer = player.Get<Components.RefillCoyoteComponent>()?.refillCoyoteTimer;
        if (refillTimer != null && LeniencyHelperModule.Session.Tweaks["RefillDashInCoyote"].Enabled 
            && refillTimer > player.dashRefillCooldownTimer
            && Input.Jump.bufferCounter > player.dashRefillCooldownTimer)
        {
            return false;
        }

        if (!SettingMaster.GetSetting<bool>("forceWaitForRefill"))
        {
            int saveDashes = player.Dashes;
            if(player.Inventory.NoRefills
                || (Math.Round(player.dashRefillCooldownTimer+Engine.DeltaTime,3) >= Math.Round(Input.Jump.bufferCounter, 3))
                || (!player.OnGround(player.Position + player.dashRefillCooldownTimer * 
                   (player.Position - player.PreviousPosition)*Engine.DeltaTime, 4))
                || (LeniencyHelperModule.Session.dashTimer > 0f)
                || (player.dashRefillCooldownTimer <= 0f && !player.RefillDash()))
            {
                player.Dashes = saveDashes;
                return true;
            }
            player.Dashes = saveDashes;
        }
        return false;
    }

    private static void CheckForDashStart(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);
        var s = LeniencyHelperModule.Session;

        if (!s.Tweaks["BufferableExtends"].Enabled) return;


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