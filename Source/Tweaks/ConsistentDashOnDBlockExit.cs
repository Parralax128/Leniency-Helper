using Monocle;
using Microsoft.Xna.Framework;
using System;
using MonoMod.Cil;
using Celeste.Mod.LeniencyHelper.Module;
using System.Windows;
using System.Threading;
using System.Diagnostics;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class ConsistentDashOnDBlockExit : AbstractTweak<ConsistentDashOnDBlockExit>
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.DreamDashUpdate += InstantDBlockExit;
        On.Celeste.Player.DreamDashEnd += DetectDreamDashEnd;
        LeniencyHelperModule.OnPlayerUpdate += MoveToDBlock;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.DreamDashUpdate -= InstantDBlockExit;
        On.Celeste.Player.DreamDashEnd -= DetectDreamDashEnd;
        LeniencyHelperModule.OnPlayerUpdate -= MoveToDBlock;
    }

    private static void DetectDreamDashEnd(On.Celeste.Player.orig_DreamDashEnd orig, Player self)
    {
        orig(self);

        var s = LeniencyHelperModule.Session;
        if(Enabled) s.dreamDashEnded = true;
    }
    private static void MoveToDBlock(Player player)
    {
        if (Enabled && LeniencyHelperModule.Session.dreamDashEnded)
        {
            int negSign = -Math.Sign(player.Speed.X);
            for (int c = 0; c < Math.Abs(player.Speed.X * Engine.DeltaTime * 2f); c++)
            {
                if (player.CollideCheck<DreamBlock>(player.Position + Vector2.UnitX * (c * negSign)))
                {
                    player.MoveHExact(Math.Max(c - 4, 0 )* negSign);
                    break;
                }
            }

            LeniencyHelperModule.Session.dreamDashEnded = false;
        }
    }
    private static float ZeroIfEnabled(float orig)
    {
        if (!Enabled) return orig;
        else return 0f;
    }

    private static void ResetDashCDifEnabled(Player player)
    {
        if(Enabled && GetSetting<bool>("resetDashCDonLeave"))
        {
            player.dashCooldownTimer = 0f;
        }
    }
    private static void InstantDBlockExit(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
            
        if(cursor.TryGotoNext(MoveType.After, 
            instr => instr.MatchStfld<Player>("dreamDashCanEndTimer"),
            instr => instr.MatchLdarg0(),
            instr => instr.MatchCall<Entity>("CollideFirst")))
        {
            if(cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld<Player>("dreamDashCanEndTimer"),
                instr => instr.MatchLdcR4(0f),
                instr => instr.MatchBgtUn(out ILLabel label)))
            {
                cursor.GotoPrev(MoveType.After, instr => instr.MatchLdfld<Player>("dreamDashCanEndTimer"));
                cursor.EmitDelegate(ZeroIfEnabled);
            }
        }

        if(cursor.TryGotoNext(MoveType.Before,
            instr => instr.MatchLdcI4(0),
            instr => instr.MatchRet()))
        {
            cursor.Index++;
            cursor.EmitLdarg0();
            cursor.EmitDelegate(ResetDashCDifEnabled);
        }
    }
}