using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;
public class DisableForcemovedTech : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.NormalUpdate += ChangeClimbjumpCheckDir;
        IL.Celeste.Player.DashUpdate += ChangeClimbjumpCheckDir;
        IL.Celeste.Player.SuperJump += UnforceAllFacings;
        IL.Celeste.Player.Throw += UnforceAllFacings;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.NormalUpdate -= ChangeClimbjumpCheckDir;
        IL.Celeste.Player.DashUpdate -= ChangeClimbjumpCheckDir;
        IL.Celeste.Player.SuperJump -= UnforceAllFacings;
        IL.Celeste.Player.Throw -= UnforceAllFacings;
    }
    private static void ChangeClimbjumpCheckDir(ILContext il)
    {
        ILCursor c = new ILCursor(il);

        while (c.TryGotoNext(MoveType.Before, i => i.MatchCallvirt<Player>("ClimbJump")))
        {
            if (c.TryGotoPrev(MoveType.After, i => i.MatchLdfld<Player>("Facing")))
                c.EmitDelegate(UnforcedFacing);

            c.GotoNext(MoveType.After, i => i.MatchCallvirt<Player>("ClimbJump"));
        }
    }
    private static void UnforceAllFacings(ILContext il)
    {
        ILCursor c = new ILCursor(il);
        while (c.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("Facing")))
        {
            c.EmitDelegate(UnforcedFacing);
        }
    }

    private static Facings UnforcedFacing(Facings orig)
    {
        if (!Enabled("DisableForcemovedTech")) return orig;
        return Input.MoveX.Value != 0 ? (Facings)Input.MoveX.Value : orig;
    }
}