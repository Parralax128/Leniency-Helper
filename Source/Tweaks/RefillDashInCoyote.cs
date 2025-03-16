using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Monocle;
using Celeste.Mod.Helpers;
using Celeste.Mod.LeniencyHelper.Components;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class RefillDashInCoyote
{
    private static ILHook origUpdateHook;
    public static void LoadHooks()
    {
        origUpdateHook = new ILHook(typeof(Player).GetMethod(nameof(Player.orig_Update)), HookedUpdate);
    }
    public static void UnloadHooks()
    {
        origUpdateHook.Dispose();
    }

    private static void HookedUpdate(ILContext il) //tl;dr we goto dash refill check as if player.dashRefillCDtimer was >0f
    {
        ILCursor c = new ILCursor(il);

        ILLabel gotoRefillCheck = il.DefineLabel();
        ILLabel goOutPreventRefill = il.DefineLabel();
        ILLabel trueRefill = il.DefineLabel();

        if(c.TryGotoNextBestFit(MoveType.After,
            instr => instr.MatchLdsfld<SaveData>("Instance"),
            instr => instr.MatchLdflda<SaveData>("Assists"),
            instr => instr.MatchLdfld<Assists>("Invincible"),
            instr => instr.MatchBrfalse(out goOutPreventRefill),
            instr => instr.MatchLdarg0(),
            instr => instr.MatchCallvirt<Player>("RefillDash")))
        {
            c.GotoPrev(MoveType.Before, instr => instr.MatchCallvirt<Player>("RefillDash"));
            c.EmitDelegate(SetTimerAndPreventRefillOnCheck);
            c.EmitBrtrue(goOutPreventRefill);
            c.EmitLdarg0();

            if (c.TryGotoPrev(MoveType.Before, instr => instr.MatchLdarg0(), instr => instr.MatchLdfld<Player>("onGround")))
            {
                c.MarkLabel(gotoRefillCheck); // represents label of the start of refill check

                if (c.TryGotoPrevBestFit(MoveType.After,
                    instr => instr.MatchCall<Engine>("get_DeltaTime"),
                    instr => instr.MatchSub(),
                    instr => instr.MatchStfld<Player>("dashRefillCooldownTimer")))
                {
                    c.EmitDelegate(StartChecking);
                    c.EmitBrtrue(gotoRefillCheck);

                    if (c.TryGotoPrevBestFit(MoveType.After,
                        instr => instr.MatchLdfld<Player>("dashRefillCooldownTimer"),
                        instr => instr.MatchLdcR4(0f),
                        instr => instr.MatchBleUn(out trueRefill)))
                    {
                        c.GotoLabel(trueRefill);
                        c.EmitDelegate(CancelArtificialCheck);
                    }
                }
            }
        }
    }
    private static bool StartChecking()
    {
        var s = LeniencyHelperModule.Session;
        if (s.TweaksEnabled["RefillDashInCoyote"])
        {
            s.artificialChecking = true;
            return true;
        }
        else return false;
    }
    private static void CancelArtificialCheck()
    {
        var s = LeniencyHelperModule.Session;
        if (s.TweaksEnabled["RefillDashInCoyote"])
            s.artificialChecking = false;
    }
    private static bool SetTimerAndPreventRefillOnCheck(Player player)
    {
        if (LeniencyHelperModule.Session.artificialChecking && LeniencyHelperModule.Session.TweaksEnabled["RefillDashInCoyote"])
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
}