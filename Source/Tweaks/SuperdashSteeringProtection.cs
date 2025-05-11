using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using VivHelper;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class SuperdashSteeringProtection : AbstractTweak
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

        if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchCall<Vector2>("Dot")))
        {
            cursor.EmitDelegate(ReturnFakeAngle);
        }
    }

    private static float ReturnFakeAngle(float orig)
    {
        if (Enabled("SuperdashSteeringProtection"))
        {
            return 0.5f;
        }
        return orig;
    }
}
