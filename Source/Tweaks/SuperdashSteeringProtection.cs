using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class SuperdashSteeringProtection : AbstractTweak<SuperdashSteeringProtection>
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.DashUpdate += DisableAngleRestriction;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.DashUpdate -= DisableAngleRestriction;
    }
    private static void DisableAngleRestriction(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        ILLabel skipCondition = il.DefineLabel();

        if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchCall<Vector2>("Dot")))
        {
            cursor.EmitDup();
            cursor.Index++;
            cursor.EmitDelegate(CheckConditionSkip);
            cursor.EmitBrtrue(skipCondition);

            cursor.GotoNextBestFit(MoveType.Before,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("Speed"),
                instr => instr.MatchLdloc0());
            cursor.MarkLabel(skipCondition);
        }
    }
    private static bool CheckConditionSkip(float cos) => Enabled && cos < 0.99f;
}
