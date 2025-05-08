using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Monocle;
using Celeste.Mod.Helpers;
using Celeste.Mod.LeniencyHelper.Components;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class RefillDashInCoyote : AbstractTweak
{
    private static ILHook origUpdateHook;

    [OnLoad]
    public static void LoadHooks()
    {
        origUpdateHook = new ILHook(typeof(Player).GetMethod(nameof(Player.orig_Update)), HookedUpdate);
        IL.Celeste.Player.Jump += CancelRefillOnJump;
        IL.Celeste.Player.SuperJump += CancelRefillOnJump;
        IL.Celeste.Player.ClimbJump += CancelRefillOnJump;
        IL.Celeste.Player.WallJump += CancelRefillOnJump;
        IL.Celeste.Player.SuperWallJump += CancelRefillOnJump;
        IL.Celeste.Player.HiccupJump += CancelRefillOnJump;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        origUpdateHook.Dispose();
        IL.Celeste.Player.Jump -= CancelRefillOnJump;
        IL.Celeste.Player.SuperJump -= CancelRefillOnJump;
        IL.Celeste.Player.ClimbJump -= CancelRefillOnJump;
        IL.Celeste.Player.WallJump -= CancelRefillOnJump;
        IL.Celeste.Player.SuperWallJump -= CancelRefillOnJump;
        IL.Celeste.Player.HiccupJump -= CancelRefillOnJump;
    }

    private static void HookedUpdate(ILContext il) //tl;dr we goto dash refill check as if player.dashRefillCDtimer was <= 0f
    {
        ILCursor cursor = new ILCursor(il);

        ILLabel gotoRefillCheck = il.DefineLabel();
        ILLabel goOutPreventRefill = il.DefineLabel();
        ILLabel trueRefill = il.DefineLabel();

        if(cursor.TryGotoNextBestFit(MoveType.After,
            instr => instr.MatchLdsfld<SaveData>("Instance"),
            instr => instr.MatchLdflda<SaveData>("Assists"),
            instr => instr.MatchLdfld<Assists>("Invincible"),
            instr => instr.MatchBrfalse(out goOutPreventRefill),
            instr => instr.MatchLdarg0(),
            instr => instr.MatchCallvirt<Player>("RefillDash")))
        {
            cursor.GotoPrev(MoveType.Before, instr => instr.MatchCallvirt<Player>("RefillDash"));
            cursor.EmitDelegate(OnRefillCheck);
            cursor.EmitBrtrue(goOutPreventRefill);
            cursor.EmitLdarg0();
            
            if (cursor.TryGotoPrev(MoveType.Before, instr => instr.MatchLdarg0(), instr => instr.MatchLdfld<Player>("onGround")))
            {
                cursor.MarkLabel(gotoRefillCheck); // represents label of the start of refill check

                if (cursor.TryGotoPrevBestFit(MoveType.After,
                    instr => instr.MatchCall<Engine>("get_DeltaTime"),
                    instr => instr.MatchSub(),
                    instr => instr.MatchStfld<Player>("dashRefillCooldownTimer")))
                {
                    cursor.EmitDelegate(StartChecking);
                    cursor.EmitBrtrue(gotoRefillCheck);

                    if (cursor.TryGotoPrevBestFit(MoveType.After,
                        instr => instr.MatchLdfld<Player>("dashRefillCooldownTimer"),
                        instr => instr.MatchLdcR4(0f),
                        instr => instr.MatchBleUn(out trueRefill)))
                    {
                        cursor.GotoLabel(trueRefill);
                        cursor.EmitDelegate(CancelArtificialCheck);
                    }
                }
            }
        }
    }
    private static bool StartChecking()
    {
        if (Enabled("RefillDashInCoyote"))
        {
            LeniencyHelperModule.Session.artificialChecking = true;
            return true;
        }
        else return false;
    }
    private static void CancelArtificialCheck()
    {
        if (Enabled("RefillDashInCoyote"))
            LeniencyHelperModule.Session.artificialChecking = false;
    }
    private static bool OnRefillCheck(Player player)
    {
        if (LeniencyHelperModule.Session.artificialChecking && Enabled("RefillDashInCoyote"))
        {
            RefillCoyoteComponent component = player.Get<RefillCoyoteComponent>();
            int saveDashes = player.Dashes;

            if (component != null && player.RefillDash())
            {
                component.ResetTimer();
                player.Dashes = saveDashes;
                return true;
            }
            player.Dashes = saveDashes;
        }
        
        return false;
    }
    private static void CancelRefillOnJump(ILContext il)
    {
        ILCursor c = new ILCursor(il);
        c.EmitLdarg0();
        c.EmitDelegate(CallComponentCancel);
    }
    private static void CallComponentCancel(Player player)
    {
        player.Get<RefillCoyoteComponent>()?.Cancel();
    }
}