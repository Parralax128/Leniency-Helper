using Monocle;
using Microsoft.Xna.Framework;
using System;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class ConsistentDashOnDBlockExit
{
    public static void LoadHooks()
    {
        IL.Celeste.Player.DreamDashUpdate += InstantDBlockExit;
        On.Celeste.Player.DreamDashBegin += ProtectInstaExit;
        On.Celeste.Player.DreamDashEnd += DetectDreamDashEnd;
        On.Celeste.Player.Update += MoveToDBlock;
    }
    public static void UnloadHooks()
    {
        IL.Celeste.Player.DreamDashUpdate -= InstantDBlockExit;
        On.Celeste.Player.DreamDashBegin -= ProtectInstaExit;
        On.Celeste.Player.DreamDashEnd -= DetectDreamDashEnd;
        On.Celeste.Player.Update -= MoveToDBlock;
    }
    
    private static void DetectDreamDashEnd(On.Celeste.Player.orig_DreamDashEnd orig, Player self)
    {
        orig(self);

        var s = LeniencyHelperModule.Session;
        if(s.TweaksEnabled["ConsistentDashOnDBlockExit"]) s.dreamDashEnded = true;
    }
    private static void MoveToDBlock(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);

        if (LeniencyHelperModule.Session.TweaksEnabled["ConsistentDashOnDBlockExit"] && LeniencyHelperModule.Session.dreamDashEnded)
        {
            int negSign = -Math.Sign(self.Speed.X);
            for (int c = 0; c < Math.Abs(self.Speed.X * Engine.DeltaTime * 2f); c++)
            {
                if (self.CollideCheck<DreamBlock>(self.Position + Vector2.UnitX * (c * negSign)))
                {
                    self.MoveHExact(Math.Max(c - 4, 0 )* negSign);
                    break;
                }
            }

            LeniencyHelperModule.Session.dreamDashEnded = false;
        }
    }
    
    private static void ProtectInstaExit(On.Celeste.Player.orig_DreamDashBegin orig, Player self)
    {
        orig(self);
        if (LeniencyHelperModule.Session.TweaksEnabled["ConsistentDashOnDBlockExit"]) LeniencyHelperModule.Session.justEntered = true;
    }
    private static float ZeroIfEnabled(float orig)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["ConsistentDashOnDBlockExit"]) return orig;
        else return 0f;
    }

    private static void ResetDashCDifEnabled(Player player)
    {
        if(LeniencyHelperModule.Session.TweaksEnabled["ConsistentDashOnDBlockExit"] &&
            (bool)LeniencyHelperModule.Settings.GetSetting("ConsistentDashOnDBlockExit", "resetDashCDonLeave"))
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