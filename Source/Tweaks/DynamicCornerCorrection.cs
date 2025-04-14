using Monocle;
using System;
using MonoMod.Cil;
using Celeste.Mod.Helpers;
using static Celeste.Mod.Helpers.ILCursorExtensions;
using static Celeste.Mod.LeniencyHelper.SettingMaster;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class DynamicCornerCorrection
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.OnCollideH += CustomOnCollideH;
        IL.Celeste.Player.OnCollideV += CustomOnCollideV;
        IL.Celeste.Player.DashUpdate += CustomJumpThruCorrection;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.OnCollideH -= CustomOnCollideH;
        IL.Celeste.Player.OnCollideV -= CustomOnCollideV;
        IL.Celeste.Player.DashUpdate -= CustomJumpThruCorrection;
    }
    private static int GetDynamicCorrection(int defaultValue, Player player, bool vertical)
    {
        if (!LeniencyHelperModule.Session.Tweaks["DynamicCornerCorrection"].Enabled ||
            (Math.Abs(player.DashDir.X) > 0.01f && Math.Abs(player.DashDir.Y) > 0.01f))
        {
            LeniencyHelperModule.Session.savedCornerCorrection = defaultValue;
            return defaultValue;
        }

        float resultingTime;
        if (vertical)
        {
            resultingTime = GetSetting<bool>("ccorectionTimingInFrames") ?
                GetSetting<float>("FloorCorrectionTiming") / 2f / Engine.FPS : GetSetting<float>("FloorCorrectionTiming") / 2f;
        }
        else
        {
            resultingTime = GetSetting<bool>("ccorectionTimingInFrames")  ?
                GetSetting<float>("WallCorrectionTiming") / Engine.FPS : GetSetting<float>("WallCorrectionTiming");
        }

        float maxSpeedY = Math.Abs(player.Speed.Y);

        if((new int[] { 2, 4, 5 }).Contains(player.StateMachine.State))
            maxSpeedY = Math.Max(Math.Abs(player.Speed.Y), Math.Abs(player.beforeDashSpeed.Y));

        float maxSpeedX = Math.Abs(player.Speed.X);
        if((new int[] { 2, 4, 5 }).Contains(player.StateMachine.State))
            maxSpeedX = Math.Max(Math.Abs(player.Speed.X), Math.Abs(player.beforeDashSpeed.X));

        LeniencyHelperModule.Session.savedCornerCorrection = 
            Math.Max((int)((vertical ? maxSpeedY : maxSpeedX) * resultingTime), defaultValue);

        return Math.Max((int)((vertical ? maxSpeedY : maxSpeedX) * resultingTime), defaultValue);
    }
    public static void CustomOnCollideH(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        if (cursor.TryGotoNextBestFit(MoveType.After,
            instr => instr.MatchLdloc(out int a),
            instr => instr.MatchLdcI4(4),
            instr => instr.MatchBle(out ILLabel label)))
        {
            cursor.Index--;

            cursor.EmitLdarg0();
            cursor.EmitLdcI4(1);
            cursor.EmitDelegate(GetDynamicCorrection);
        }
    }
    public static void CustomOnCollideV(ILContext il)
    {
        
        ILCursor cursor = new ILCursor(il);

        if (cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchCall<Actor>("MoveVExact"))) 
        {
            if (cursor.TryGotoNext(MoveType.Before,
                instr => instr.MatchBge(out ILLabel label)))
            {
                cursor.EmitLdarg0();
                cursor.EmitLdcI4(0);
                cursor.EmitDelegate(GetDynamicCorrection);
                cursor.EmitNeg();
            }

        }

        if (cursor.TryGotoNextBestFit(MoveType.After,
            instr => instr.MatchAdd(),
            instr => instr.MatchStloc(out int a),
            instr => instr.MatchLdloc(out int a),
            instr => instr.MatchLdcI4(4),
            instr => instr.MatchBle(out ILLabel label)))
        {
            cursor.Index--;

            cursor.EmitLdarg0();
            cursor.EmitLdcI4(0);
            cursor.EmitDelegate(GetDynamicCorrection);
        }

        for(int c=0; c<2; c++)
        {
            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchAdd(),
                instr => instr.MatchStloc(out int a),
                instr => instr.MatchLdloc(out int a),
                instr => instr.MatchLdloc(out int a),
                instr => instr.MatchBle(out ILLabel label)))
            {
                cursor.Index--;

                cursor.EmitLdarg0();
                cursor.EmitLdcI4(0);
                cursor.EmitDelegate(GetDynamicCorrection);
            }
        }
    }
    private static float DoubleAbs(float value)
    {
        if (LeniencyHelperModule.Session.Tweaks["DynamicCornerCorrection"].Enabled) return (value < 0f ? value * -2f : value);
        else return value;
    }
    private static void CustomJumpThruCorrection(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        ILLabel skipBoundsCheck = il.DefineLabel();

        if (cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchCastclass<JumpThru>(),
            instr => instr.MatchStloc(out int a),
            instr => instr.MatchLdarg0()))
        {
            cursor.GotoNext(MoveType.After, instr => instr.MatchSub(), instr => instr.MatchLdcR4(6f));
            cursor.Index--;

            cursor.EmitDelegate(DoubleAbs);

            if (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchBgtUn(out ILLabel label)))
            {
                cursor.EmitConvI4();
                cursor.EmitLdarg0();
                cursor.EmitLdcI4(1);
                cursor.EmitDelegate(GetDynamicCorrection);
                cursor.EmitConvR4();


                cursor.GotoPrev(MoveType.After, instr => instr.MatchLdloc(out int a), instr => instr.MatchCall<Entity>("CollideCheck"));

                cursor.EmitDelegate(CheckEnabled);
                cursor.EmitBrfalse(skipBoundsCheck);

                cursor.EmitPop();
                cursor.EmitLdloc(6);
                cursor.EmitLdarg0();
                cursor.EmitDelegate(InBounds);

                cursor.MarkLabel(skipBoundsCheck);
            }
        }
    }

    private static bool CheckEnabled()
    {
        return LeniencyHelperModule.Session.Tweaks["DynamicCornerCorrection"].Enabled;
    }
    private static bool InBounds(Entity entity, Player player)
    {
        return (player.Right >= entity.Left && player.Left <= entity.Right);
    }
}