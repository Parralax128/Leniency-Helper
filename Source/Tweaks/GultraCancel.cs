using System;
using Celeste.Mod.Helpers;
using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class GultraCancel : AbstractTweak
{
    private static ILHook modifyDashCoroutine;
    private static ILContext.Manipulator onCollideVHook = (ILContext il) => AddSpeedPreservation(il, false);
    private static ILContext.Manipulator dashCoroutineHook = (ILContext il) => AddSpeedPreservation(il, true);

    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.OnCollideV += onCollideVHook;
        modifyDashCoroutine = new ILHook(typeof(Player).GetMethod("DashCoroutine",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetStateMachineTarget(),
            dashCoroutineHook);
        On.Celeste.Player.DashUpdate += CancelGultraOnMidAir;
        On.Celeste.Player.DashBegin += ClearSavedSpeed;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.OnCollideV -= onCollideVHook;
        On.Celeste.Player.DashUpdate -= CancelGultraOnMidAir;
        On.Celeste.Player.DashBegin -= ClearSavedSpeed;
        modifyDashCoroutine.Dispose();
    }

    private static void ClearSavedSpeed(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);
        LeniencyHelperModule.Session.savedSpeed = null;
    }
    private static void AddSpeedPreservation(ILContext il, bool coroutine)
    {
        ILCursor cursor = new ILCursor(il);

        if(cursor.TryGotoNext(MoveType.Before, 
            instr => instr.MatchLdcI4(1),
            instr => instr.MatchCallvirt<Player>("set_Ducking")))
        {
            if(cursor.TryGotoPrevBestFit(MoveType.Before,
                instr => instr.MatchLdflda<Player>("Speed"),
                instr => instr.MatchLdcR4(0f),
                instr => instr.MatchStfld<Vector2>("Y")))
            {
                cursor.Index--;

                if (coroutine) cursor.EmitLdloc1();
                else cursor.EmitLdarg0();

                cursor.EmitDelegate(SaveSpeedY);
            }
        }
    }
    private static void SaveSpeedY(Player player)
    {
        LeniencyHelperModule.Session.savedSpeed = player.Speed;
    }

    private static int CancelGultraOnMidAir(On.Celeste.Player.orig_DashUpdate orig, Player self)
    {
        if (Enabled("GultraCancel") && LeniencyHelperModule.Session.savedSpeed.HasValue
            && !self.OnGround() && self.DashDir.Y == 0f && self.Speed.Y == 0f)
        {
            self.Speed = LeniencyHelperModule.Session.savedSpeed.Value;
            self.DashDir = new Vector2(Math.Sign(self.Speed.X), Math.Sign(self.Speed.Y)).SafeNormalize();
            self.Ducking = false;
        }
        return orig(self);
    }
}