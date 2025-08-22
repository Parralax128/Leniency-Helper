using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CustomSnapDownDistance : AbstractTweak<CustomSnapDownDistance>
{
    private static ILHook origUpdateHook;

    [OnLoad]
    public static void LoadHooks()
    {
        origUpdateHook = new ILHook(typeof(Player).GetMethod("orig_Update"), ModifySnapdownDistance);
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        origUpdateHook.Dispose();
    }

    private static float GetCustomDistance(float defaultValue, Player player)
    {
        if (!Enabled) return defaultValue;

        if (GetSetting<bool>("dynamicSnapdownDistance"))
        {
            return (int)Math.Max(defaultValue, Math.Max(Math.Abs(player.beforeDashSpeed.Y),
                Math.Abs(player.Speed.Y)) * GetTime("snapdownTiming"));
        }
        else
        {
            return GetSetting<int>("staticSnapdownDistance");
        }
    }

    private static void ModifySnapdownDistance(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt<Player>("DashCorrectCheck")))
        {
            if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdnull()))
            {
                cursor.EmitConvR4();
                cursor.EmitLdarg0();
                cursor.EmitDelegate(GetCustomDistance);
                cursor.EmitConvI4();
            }

            for(int c=0; c<3; c++)
            {
                if (cursor.TryGotoPrev(MoveType.Before, instr => instr.MatchCall<Microsoft.Xna.Framework.Vector2>("op_Multiply")))
                {
                    cursor.EmitLdarg0();
                    cursor.EmitDelegate(GetCustomDistance);
                }
            }
        }
    }
}