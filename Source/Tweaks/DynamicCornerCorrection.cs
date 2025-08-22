using Monocle;
using System;
using MonoMod.Cil;
using Celeste.Mod.Helpers;
using static Celeste.Mod.Helpers.ILCursorExtensions;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class DynamicCornerCorrection : AbstractTweak<DynamicCornerCorrection>
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
    public static int GetDynamicCorrection(int defaultValue, Player player, bool vertical)
    {
        defaultValue = (int)Math.Abs(defaultValue);
        if (!Enabled || (Math.Abs(player.DashDir.X) > 0.2f && Math.Abs(player.DashDir.Y) > 0.2f))
        {
            LeniencyHelperModule.Session.cornerCorrection = vertical ?
                LeniencyHelperModule.Session.cornerCorrection with { Y = defaultValue } :
                LeniencyHelperModule.Session.cornerCorrection with { X = defaultValue };

            return defaultValue;
        }

        float resultingTime = GetSetting<float>(vertical ? "FloorCorrectionTiming" : "WallCorrectionTiming") *
                (GetSetting<bool>("ccorectionTimingInFrames") ? Engine.DeltaTime : 1f);

        float maxSpeed = Math.Abs(vertical ? player.Speed.Y : player.Speed.X);
        if ((new int[] { 2, 4, 5 }).Contains(player.StateMachine.State))
        {
            maxSpeed = Math.Max(Math.Abs(vertical ? player.Speed.Y : player.Speed.X), 
                Math.Abs(vertical ? player.beforeDashSpeed.Y : player.beforeDashSpeed.X));
        }

        int result = Math.Max((int)(maxSpeed * resultingTime), defaultValue);

        LeniencyHelperModule.Session.cornerCorrection = vertical ?
                LeniencyHelperModule.Session.cornerCorrection with { Y = result } :
                LeniencyHelperModule.Session.cornerCorrection with { X = result };

        return result;
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
    private static float DoubleAbs(float orig) => Enabled ? (orig < 0f ? orig * -2f : orig) : orig;
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

                cursor.EmitDelegate(() => Enabled);
                cursor.EmitBrfalse(skipBoundsCheck);

                cursor.EmitPop();
                cursor.EmitLdloc(6);
                cursor.EmitLdarg0();
                cursor.EmitDelegate(InBounds);

                cursor.MarkLabel(skipBoundsCheck);
            }
        }
    }
    private static bool InBounds(Entity entity, Player player)
    {
        return (player.Right >= entity.Left && player.Left <= entity.Right);
    }
}