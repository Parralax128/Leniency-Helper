using Celeste.Mod.LeniencyHelper.Module;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class ManualDreamhyperLeniency : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.DashUpdate += RemoveDashDirCheck;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.DashUpdate -= RemoveDashDirCheck;
    }
    private static void RemoveDashDirCheck(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        ILLabel gotoSuper = il.DefineLabel();
        ILLabel skip = null;
        VariableDefinition Checked = new VariableDefinition(il.Import(typeof(bool)));
        il.Body.Variables.Add(Checked);

        cursor.EmitLdcI4(0);
        cursor.EmitStloc(Checked);

        if (cursor.TryGotoNext(MoveType.After, 
            instr => instr.MatchLdcR4(0.1f),
            instr => instr.MatchBgeUn(out skip)))
        {
            cursor.GotoNext(MoveType.After, instr => instr.MatchEndfinally());
            cursor.MarkLabel(gotoSuper);


            cursor.GotoLabel(skip);
            cursor.Index++;
            cursor.EmitLdloc(Checked);
            cursor.EmitDelegate(DreamHyperCheck);

            cursor.EmitLdcI4(1);
            cursor.EmitStloc(Checked);

            cursor.EmitBrtrue(gotoSuper);

            cursor.EmitLdarg0();
        }
    }

    private static bool DreamHyperCheck(Player player, bool alreadyChecked)
    {
        return Enabled("ManualDreamhyperLeniency") && !alreadyChecked && player.DashDir.Y > 0f && player.DashDir.X != 0f;
    }
}
