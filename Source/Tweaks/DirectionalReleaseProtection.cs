using Monocle;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using Celeste.Mod.Helpers;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class DirectionalReleaseProtection
{
    private static Vector2 bufferAimTimer;
    private static Vector2 savedAimValue;
    private static Vector2 bufferMoveTimer;
    private static Vector2 savedMoveValue;

    private static ILHook DashCoroutineHook;
    private static ILHook BoosterCoroutineHook;
    private static ILHook RedBoosterCoroutineHook;
    public static void LoadHooks()
    {
        On.Celeste.Player.Jump += JumpDirBuffer;
        On.Celeste.Player.SuperJump += SuperJumpDirBuffer;
        On.Celeste.Player.WallJump += WallJumpDirBuffer;
        On.Celeste.Player.ClimbJump += ClimbJumpDirBuffer;

        DashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), DashDirBuffer);
        BoosterCoroutineHook = new ILHook(typeof(Player).GetMethod("BoostCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), DashDirBuffer);
        RedBoosterCoroutineHook = new ILHook(typeof(Player).GetMethod("RedDashCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), DashDirBuffer);

        IL.Monocle.Engine.Update += UpdateOnFF;
    }
    public static void UnloadHooks()
    {
        On.Celeste.Player.Jump -= JumpDirBuffer;
        On.Celeste.Player.SuperJump -= SuperJumpDirBuffer;
        On.Celeste.Player.WallJump -= WallJumpDirBuffer;
        On.Celeste.Player.ClimbJump -= ClimbJumpDirBuffer;
        IL.Celeste.Player.DashCoroutine -= DashDirBuffer;

        DashCoroutineHook.Dispose();
        BoosterCoroutineHook.Dispose();
        RedBoosterCoroutineHook.Dispose();

        IL.Monocle.Engine.Update -= UpdateOnFF;
    }
    private static void UpdateOnFF(ILContext il)
    {
        ILCursor c = new ILCursor(il);

        if (c.TryGotoNextBestFit(MoveType.Before,
            instr => instr.MatchLdsfld<Engine>("FreezeTimer"),
            instr => instr.MatchLdcR4(0),
            instr => instr.MatchBleUn(out ILLabel l)))
        {
            c.EmitDelegate(UpdateDirectionalBuffers);
        }
    }

    private static float ActualBufferTime =>
        ((bool)LeniencyHelperModule.Settings.GetSetting("DirectionalReleaseProtection", "CountProtectionTimeInFrames") ?
        (float)LeniencyHelperModule.Settings.GetSetting("DirectionalReleaseProtection", "DirectionalBufferTime") / Engine.FPS :
        (float)LeniencyHelperModule.Settings.GetSetting("DirectionalReleaseProtection", "DirectionalBufferTime"));

    private static void UpdateDirectionalBuffers()
    {
        var s = LeniencyHelperModule.Session;
        if (s == null || !LeniencyHelperModule.Session.TweaksEnabled["DirectionalReleaseProtection"]) return;


        Dirs? dashDir = (Dirs?)LeniencyHelperModule.Settings.GetSetting("DirectionalReleaseProtection", "dashDir");
        if (dashDir is null) return;

        if (bufferAimTimer.X > 0f) bufferAimTimer.X -= Engine.RawDeltaTime;
        if (bufferAimTimer.Y > 0f) bufferAimTimer.Y -= Engine.RawDeltaTime;
        if (bufferMoveTimer.X > 0f) bufferMoveTimer.X -= Engine.RawDeltaTime;
        if (bufferMoveTimer.Y > 0f) bufferMoveTimer.Y -= Engine.RawDeltaTime;

        //X
        if (Input.Aim.Value.X > 0f && dashDir == Dirs.Right || Input.Aim.Value.X < 0f && dashDir == Dirs.Left)
        {
            bufferAimTimer.X = ActualBufferTime;
            savedAimValue.X = Input.Aim.Value.X;
        }

        //Y
        if (Input.Aim.Value.Y > 0f && dashDir == Dirs.Down || Input.Aim.Value.Y < 0f && dashDir == Dirs.Up)
        {
            bufferAimTimer.Y = ActualBufferTime;
            savedAimValue.Y = Input.Aim.Value.Y;
        }

        Dirs? jumpDir = (Dirs?)LeniencyHelperModule.Settings.GetSetting("DirectionalReleaseProtection", "jumpDir");
        if (s is null || jumpDir is null || jumpDir == Dirs.None) return;

        bool posX, posY;
        bool negX, negY;

        Input.MoveX.CheckBinds(out posX, out negX);
        Input.MoveY.CheckBinds(out posY, out negY);

        if (posX && (jumpDir == Dirs.Right || jumpDir == Dirs.All) || negX && (jumpDir == Dirs.Left || jumpDir == Dirs.All))
        {
            bufferMoveTimer.X = ActualBufferTime;
            savedMoveValue.X = Input.MoveX.Value;
        }

        if (posY && (jumpDir == Dirs.Down || jumpDir == Dirs.All) || negY && (jumpDir == Dirs.Up || jumpDir == Dirs.All))
        {
            bufferMoveTimer.Y = ActualBufferTime;
            savedMoveValue.Y = Input.MoveY.Value;
        }
    }

    private static void JumpDirBuffer(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx)
    {
        if(!LeniencyHelperModule.Session.TweaksEnabled["DirectionalReleaseProtection"])
        {
            orig(self, particles, playSfx);
            return;
        }

        int save = self.moveX;
        if (self.moveX == 0 && bufferMoveTimer.X > 0f)
        {
            self.moveX = (int)savedMoveValue.X;
        }

        orig(self, particles, playSfx);

        self.moveX = save;
    }

    private static void SuperJumpDirBuffer(On.Celeste.Player.orig_SuperJump orig, Player self)
    {
        if(!LeniencyHelperModule.Session.TweaksEnabled["DirectionalReleaseProtection"])
        {
            orig(self);
            return;
        }

        int save = Input.MoveX.Value;
        if (Input.MoveX.Value == 0 && bufferMoveTimer.X > 0f)
        {
            Input.MoveX.Value = (int)savedMoveValue.X;
        }

        orig(self);

        Input.MoveX.Value = save;
    }
    private static void WallJumpDirBuffer(On.Celeste.Player.orig_WallJump orig, Player self, int dir)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["DirectionalReleaseProtection"])
        {
            orig(self, dir);
            return;
        }

        int save = self.moveX;
        if (self.moveX == 0 && bufferMoveTimer.X > 0f)
        {
            self.moveX = (int)savedMoveValue.X;
        }

        orig(self, dir);

        self.moveX = save;
    }
    private static void ClimbJumpDirBuffer(On.Celeste.Player.orig_ClimbJump orig, Player self)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["DirectionalReleaseProtection"])
        {
            orig(self);
            return;
        }

        int save = self.moveX;

        if (self.moveX == 0 && bufferMoveTimer.X > 0f)
        {
            self.moveX = (int)savedMoveValue.X;
        }
        orig(self);

        self.moveX = save;
    }

    private static Vector2 ChangeLastAim(Vector2 orig)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["DirectionalReleaseProtection"])
        {
            return orig;
        }

        Vector2 result = orig;

        if (orig.X == 0 && bufferAimTimer.X > 0f)
            result.X = savedAimValue.X;
        if (orig.Y == 0 && bufferAimTimer.Y > 0f)
            result.Y = savedAimValue.Y;

        result.X = Math.Sign(result.X);
        result.Y = Math.Sign(result.Y);

        return result.SafeNormalize();
    }
    private static void DashDirBuffer(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        while(cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdfld<Player>("lastAim")))
        {
            cursor.EmitDelegate(ChangeLastAim);
        }
    }
}