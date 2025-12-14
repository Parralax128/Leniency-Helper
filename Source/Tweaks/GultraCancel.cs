using System;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class GultraCancel : AbstractTweak<GultraCancel>
{
    [SaveState] static Vector2? savedSpeed = null;
    static Timer CancelTimer = new();


    static ILHook modifyDashCoroutine;
    static ILContext.Manipulator onCollideVHook = (il) => AddSpeedPreservation(il, false);
    static ILContext.Manipulator dashCoroutineHook = (il) => AddSpeedPreservation(il, true);

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

    static void ClearSavedSpeed(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);
        savedSpeed = null;
    }

    static void AddSpeedPreservation(ILContext il, bool coroutine)
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


        static void SaveSpeedY(Player player)
        {
            savedSpeed = player.Speed;
            CancelTimer.Launch(GetSetting<Time>());
        }
    }

    static int CancelGultraOnMidAir(On.Celeste.Player.orig_DashUpdate orig, Player self)
    {
        // dash stated on ground + left ground + no vertical speed & dash dir
        if(Enabled && CancelTimer && savedSpeed.HasValue && !self.dashStartedOnGround
            && !self.OnGround() && self.DashDir.Y == 0f && self.Speed.Y == 0f)
        {
            self.Speed = savedSpeed.Value;
            self.DashDir = new Vector2(Math.Sign(self.Speed.X), Math.Sign(self.Speed.Y)).SafeNormalize();
            self.Ducking = false;
        }

        int nextState = orig(self);
        if (nextState != Player.StDash) CancelTimer.Abort();
        return nextState;
    }
}