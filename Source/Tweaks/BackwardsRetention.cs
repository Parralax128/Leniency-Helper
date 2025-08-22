using System;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Monocle;
using System.Reflection;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class BackwardsRetention : AbstractTweak<BackwardsRetention>
{
    private static ILHook origUpdateHook;
    [OnLoad]
    public static void LoadHooks()
    {
        origUpdateHook = new ILHook(typeof(Player).GetMethod(nameof(Player.orig_Update)), HookedUpdate);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        origUpdateHook.Dispose();
    }
    
    private static bool CanSkipCancel(Player player)
    {
        return Enabled ? (Math.Abs(player.Speed.X) < 40f) : false; 
        // ~40 is speed player can reach in their wallRetainTime from zero via air-movement
    }
    private static void HookedUpdate(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        ILLabel label = null;

        Type[] mathSignArgs = { typeof(float) };

        if (cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdcR4(0f),
            instr => instr.MatchStfld<Player>("wallSpeedRetentionTimer")))
        {
            if(cursor.TryGotoPrev(MoveType.After,
                instr => instr.MatchNeg(),
                instr => instr.MatchBneUn(out label)))
            {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(CanSkipCancel);
                cursor.EmitBrtrue(label);

                if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCall<Entity>("CollideCheck")))
                {
                    if (cursor.TryGotoPrev(MoveType.After,
                        instr => instr.MatchCall(typeof(Math).GetMethod(nameof(Math.Sign), BindingFlags.Public | BindingFlags.Static, mathSignArgs)),
                        instr => instr.MatchConvR4()))
                    {
                        cursor.EmitLdarg0();
                        cursor.EmitDelegate(AddCollideCheckDist);
                    }
                }
            }
        }
    }

    private static float AddCollideCheckDist(float @default, Player player)
    {
        float result = @default * (Enabled &&
            Math.Sign(player.Speed.X) == -Math.Sign(player.wallSpeedRetained) ? 4 : 1);
        
        return result;
    }
}