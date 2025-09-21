using Monocle;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using Celeste.Mod.Helpers;
using Celeste.Mod.LeniencyHelper.Module;
using YamlDotNet.Core.Tokens;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class DirectionalReleaseProtection : AbstractTweak<DirectionalReleaseProtection>
{
    private static Vector2 bufferAimTimer;
    private static Vector2 savedAimValue;
    private static Vector2 bufferMoveTimer;
    private static Vector2 savedMoveValue;

    private static float featherAimTimer;

    private static ILHook DashCoroutineHook;
    private static ILHook BoosterCoroutineHook;
    private static ILHook RedBoosterCoroutineHook;

    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Jump += JumpDirBuffer;
        On.Celeste.Player.SuperJump += SuperJumpDirBuffer;
        On.Celeste.Player.WallJump += WallJumpDirBuffer;
        On.Celeste.Player.ClimbJump += ClimbJumpDirBuffer;

        On.Celeste.Player.StarFlyUpdate += AffectSavedFeatherDir;
        IL.Celeste.Player.DashUpdate += AffectSuperDasheDir;

        DashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), DashDirBuffer);

        BoosterCoroutineHook = new ILHook(typeof(Player).GetMethod("BoostCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), DashDirBuffer);

        RedBoosterCoroutineHook = new ILHook(typeof(Player).GetMethod("RedDashCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), DashDirBuffer);

        IL.Monocle.Engine.Update += UpdateOnFF;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Jump -= JumpDirBuffer;
        On.Celeste.Player.SuperJump -= SuperJumpDirBuffer;
        On.Celeste.Player.WallJump -= WallJumpDirBuffer;
        On.Celeste.Player.ClimbJump -= ClimbJumpDirBuffer;
        IL.Celeste.Player.DashCoroutine -= DashDirBuffer;

        On.Celeste.Player.StarFlyUpdate -= AffectSavedFeatherDir;
        IL.Celeste.Player.DashUpdate -= AffectSuperDasheDir; // Determined buffer order!!

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

    private static float ActualBufferTime => GetSetting<bool>("CountProtectionTimeInFrames") ? 
        GetSetting<float>("DirectionalBufferTime") / Engine.FPS : GetSetting<float>("DirectionalBufferTime");

    private static void UpdateDirectionalBuffers()
    {
        if (LeniencyHelperModule.Session == null || !Enabled) return;

        if (bufferAimTimer.X > 0f) bufferAimTimer.X -= Engine.RawDeltaTime;
        if (bufferAimTimer.Y > 0f) bufferAimTimer.Y -= Engine.RawDeltaTime;
        if (bufferMoveTimer.X > 0f) bufferMoveTimer.X -= Engine.RawDeltaTime;
        if (bufferMoveTimer.Y > 0f) bufferMoveTimer.Y -= Engine.RawDeltaTime;
        if (featherAimTimer > 0f) featherAimTimer -= Engine.RawDeltaTime;

        Dirs dashDir = GetSetting<Dirs>("dashDir");
        if (dashDir != Dirs.None || GetSetting<bool>("affectFeathers") || GetSetting<bool>("affectSuperdashes"))
        {
            //X
            if (Input.Aim.Value.X > 0f && (dashDir == Dirs.Right || dashDir == Dirs.All)
                || Input.Aim.Value.X < 0f && (dashDir == Dirs.Left || dashDir == Dirs.All))
            {
                bufferAimTimer.X = ActualBufferTime;
                savedAimValue.X = Input.Aim.Value.X;
            }

            //Y
            if (Input.Aim.Value.Y > 0f && (dashDir == Dirs.Down || dashDir == Dirs.All)
                || Input.Aim.Value.Y < 0f && (dashDir == Dirs.Up || dashDir == Dirs.All))
            {
                bufferAimTimer.Y = ActualBufferTime;
                savedAimValue.Y = Input.Aim.Value.Y;
            }
        }

        Dirs jumpDir = GetSetting<Dirs>("jumpDir");
        if (jumpDir == Dirs.None) return;

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
        if(!Enabled)
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
        if(!Enabled)
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
        if (!Enabled)
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
        if (!Enabled)
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
        if (!Enabled)
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

    private static int AffectSavedFeatherDir(On.Celeste.Player.orig_StarFlyUpdate orig, Player self)
    {
        if (Enabled && GetSetting<bool>("affectFeathers"))
        {
            if (Input.Feather.Value != Vector2.Zero) featherAimTimer = ActualBufferTime;
            if(featherAimTimer > 0f)
            self.starFlyLastDir = SnapAim(self.starFlyLastDir);
        }
        
        return orig(self);
    }
    public static void AffectSuperDasheDir(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        ILLabel endLabel;

        if(cursor.TryGotoNextBestFit(MoveType.After, 
            instr => instr.MatchLdfld<Player>("canCurveDash"),
            instr => instr.MatchBrfalse(out endLabel),
            instr => instr.MatchLdsfld(typeof(Input).GetField("Aim"))))
        {
            cursor.GotoNext(MoveType.After, instr => instr.MatchCall<Vector2>("op_Inequality"));
            cursor.EmitDelegate(OrSuperdashesAffected);

            if(cursor.TryGotoNextBestFit(MoveType.Before,
                instr => instr.MatchLdloc0(),
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld<Player>("Speed"),
                instr => instr.MatchCall(typeof(Calc).GetMethod("SafeNormalize", new Type[] { typeof(Vector2) })),
                instr => instr.MatchCall<Vector2>("Dot")))
            {
                int saveIndex = cursor.Index;
                
                cursor.GotoNext(MoveType.After, instr => instr.MatchLdcR4(0.99f), instr => instr.MatchBgeUn(out ILLabel l));
                cursor.GotoNext(MoveType.Before, instr => instr.MatchLdarg0(), instr => instr.MatchLdarg0(), instr => instr.MatchLdfld<Player>("Speed"));
                ILLabel skipCondition = il.DefineLabel();
                cursor.MarkLabel(skipCondition);

                cursor.Goto(saveIndex);

                cursor.EmitDelegate(() => Input.Aim.Value == Vector2.Zero);
                cursor.EmitBrtrue(skipCondition);

                if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCall(typeof(Calc).GetMethod("Angle", new Type[] { typeof(Vector2) }))))
                {
                    cursor.EmitLdarg0();
                    cursor.EmitLdfld(typeof(Player).GetField("Speed"));
                    cursor.EmitDelegate(ModifySuperdashTarget);
                }
            }
        }
    }
    private static bool OrSuperdashesAffected(bool orig) => Enabled && GetSetting<bool>("affectSuperdashes") ? true : orig;
    private static Vector2 ModifySuperdashTarget(Vector2 orig, Vector2 speed)
        => Enabled && GetSetting<bool>("affectSuperdashes") && Input.Aim.Value == Vector2.Zero ? SnapAim(speed) : orig;
    
    private static Vector2 SnapAim(Vector2 aim)
    {
        float num = aim.Angle();
        int num2 = ((num < 0f) ? 1 : 0);
        float num3 = MathF.PI / 8f - (float)num2 * ((float)Math.PI / 36f);
        if (Calc.AbsAngleDiff(num, 0f) < num3) return new Vector2(1f, 0f);
        if (Calc.AbsAngleDiff(num, MathF.PI) < num3) return new Vector2(-1f, 0f);
        if (Calc.AbsAngleDiff(num, -MathF.PI / 2f) < num3) return new Vector2(0f, -1f);
        if (Calc.AbsAngleDiff(num, MathF.PI / 2f) < num3) return new Vector2(0f, 1f);
        
        return new Vector2(Math.Sign(aim.X), Math.Sign(aim.Y)).SafeNormalize();
    }
}