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
using Mono.Cecil.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class DirectionalReleaseProtection : AbstractTweak<DirectionalReleaseProtection>
{
    class CoupledTimer
    {
        public Timer X;
        public Timer Y;

        public Vector2 savedValue;

        public CoupledTimer()
        {
            X = new Timer(Timer.Type.Input);
            Y = new Timer(Timer.Type.Input);
        }

        public void SaveX(float value, int? settingIndex = null)
        {
            savedValue.X = value;
            X.Launch(GetSetting<Time>(settingIndex ?? ProtectionTime));
        }
        public void SaveY(float value, int? settingIndex = null)
        {
            savedValue.Y = value;
            Y.Launch(GetSetting<Time>(settingIndex ?? ProtectionTime));
        }
    }


    [SettingIndex] static int DashDir;
    [SettingIndex] static int JumpDir;
    [SettingIndex] static int ProtectionTime;
    [SettingIndex] static int AffectFeathers;
    [SettingIndex] static int AffectSuperdashes;


    static CoupledTimer AimTimer = new();
    static CoupledTimer MoveTimer = new();

    static Timer FeatherAimTimer = new(Timer.Type.Input);

    static ILHook DashCoroutineHook;
    static ILHook BoosterCoroutineHook;
    static ILHook RedBoosterCoroutineHook;

    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.Jump += ProtectMoveX;
        IL.Celeste.Player.SuperJump += ProtectMoveX;
        IL.Celeste.Player.WallJump += ProtectMoveX;
        IL.Celeste.Player.ClimbJump += ProtectMoveX;

        On.Celeste.Player.StarFlyUpdate += AffectSavedFeatherDir;
        IL.Celeste.Player.DashUpdate += AffectSuperDashDir;

        DashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), ProtectDashAim);

        BoosterCoroutineHook = new ILHook(typeof(Player).GetMethod("BoostCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), ProtectDashAim);

        RedBoosterCoroutineHook = new ILHook(typeof(Player).GetMethod("RedDashCoroutine",
            BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), ProtectDashAim);

        IL.Monocle.Engine.Update += UpdateOnFF;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.Jump -= ProtectMoveX;
        IL.Celeste.Player.SuperJump -= ProtectMoveX;
        IL.Celeste.Player.WallJump -= ProtectMoveX;
        IL.Celeste.Player.ClimbJump -= ProtectMoveX;
        IL.Celeste.Player.DashCoroutine -= ProtectDashAim;

        On.Celeste.Player.StarFlyUpdate -= AffectSavedFeatherDir;
        IL.Celeste.Player.DashUpdate -= AffectSuperDashDir;

        DashCoroutineHook.Dispose();
        BoosterCoroutineHook.Dispose();
        RedBoosterCoroutineHook.Dispose();

        IL.Monocle.Engine.Update -= UpdateOnFF;
    }


    #region dashes and jumps
    static void UpdateOnFF(ILContext il)
    {
        ILCursor c = new ILCursor(il);

        if (c.TryGotoNextBestFit(MoveType.Before,
            instr => instr.MatchLdsfld<Engine>("FreezeTimer"),
            instr => instr.MatchLdcR4(0),
            instr => instr.MatchBleUn(out ILLabel l)))
        {
            c.EmitDelegate(UpdateProtectedDirections);
        }
    }
    static void UpdateProtectedDirections()
    {
        if (Engine.Scene is not Level || LeniencyHelperModule.Session == null || !Enabled) return;

        Dirs dashDir = GetSetting<Dirs>(DashDir);
        if (dashDir != Dirs.None || GetSetting<bool>(AffectFeathers) || GetSetting<bool>(AffectSuperdashes))
        {
            //X
            if (Input.Aim.Value.X > 0f && (dashDir == Dirs.Right || dashDir == Dirs.All)
                || Input.Aim.Value.X < 0f && (dashDir == Dirs.Left || dashDir == Dirs.All))
            {
                AimTimer.SaveX(Input.Aim.Value.X);
            }

            //Y
            if (Input.Aim.Value.Y > 0f && (dashDir == Dirs.Down || dashDir == Dirs.All)
                || Input.Aim.Value.Y < 0f && (dashDir == Dirs.Up || dashDir == Dirs.All))
            {
                AimTimer.SaveY(Input.Aim.Value.Y);
            }
        }

        Dirs jumpDir = GetSetting<Dirs>(JumpDir);
        if (jumpDir == Dirs.None) return;

        Input.MoveX.CheckBinds(out bool posX, out bool negX);
        Input.MoveY.CheckBinds(out bool posY, out bool negY);

        if (posX && (jumpDir == Dirs.Right || jumpDir == Dirs.All) || negX && (jumpDir == Dirs.Left || jumpDir == Dirs.All))
        {
            MoveTimer.SaveX(Input.MoveX.Value);
        }

        if (posY && (jumpDir == Dirs.Down || jumpDir == Dirs.All) || negY && (jumpDir == Dirs.Up || jumpDir == Dirs.All))
        {
            MoveTimer.SaveY(Input.MoveY.Value);
        }
    }


    static void ProtectMoveX(ILContext il)
    {
        VariableDefinition savedMove = new(il.Import(typeof(int)));
        il.Body.Variables.Add(savedMove);

        ILCursor cursor = new(il);

        cursor.EmitLdarg0();
        cursor.EmitDelegate(ChangeMoveX);
        cursor.EmitStloc(savedMove);

        while(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchRet()))
        {
            cursor.EmitLdarg0();
            cursor.EmitLdloc(savedMove);
            cursor.EmitDelegate(Revert);

            cursor.GotoNext(MoveType.After, instr => instr.MatchRet());
        }

        static int ChangeMoveX(Player player)
        {
            int save = Input.MoveX.Value;

            if (Enabled && Input.MoveX.Value == 0 && MoveTimer.X)
                Input.MoveX.Value = player.moveX = (int)MoveTimer.savedValue.X;

            return save;
        }
        static void Revert(Player player, int saved)
        {
            if (!Enabled) return;
            Input.MoveX.Value = player.moveX = saved;
        }
    }
    
    static void ProtectDashAim(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        while(cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdfld<Player>("lastAim")))
        {
            cursor.EmitDelegate(ChangeLastAim);
        }


        static Vector2 ChangeLastAim(Vector2 orig)
        {
            if (!Enabled) return orig;

            Vector2 result = orig;

            if (orig.X == 0 && AimTimer.X) result.X = AimTimer.savedValue.X;
            if (orig.Y == 0 && AimTimer.Y) result.Y = AimTimer.savedValue.Y;

            result.X = Math.Sign(result.X);
            result.Y = Math.Sign(result.Y);

            return result.SafeNormalize();
        }
    }

    static int AffectSavedFeatherDir(On.Celeste.Player.orig_StarFlyUpdate orig, Player self)
    {
        if (Enabled && GetSetting<bool>(AffectFeathers))
        {
            if (Input.Feather.Value != Vector2.Zero) FeatherAimTimer.Launch(GetSetting<Time>(ProtectionTime));
            else if (FeatherAimTimer) self.starFlyLastDir = SnapAim(self.starFlyLastDir);
        }
        
        return orig(self);

    }

    #endregion


    public static void AffectSuperDashDir(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        VariableDefinition zeroAim = new(il.Import(typeof(bool)));
        VariableDefinition enabled = new(il.Import(typeof(bool)));

        il.Body.Variables.Add(zeroAim);
        il.Body.Variables.Add(enabled);


        if (cursor.TryGotoNextBestFit(MoveType.After,
            instr => instr.MatchLdfld<Player>("canCurveDash"),
            instr => instr.MatchBrfalse(out ILLabel l),
            instr => instr.MatchLdsfld(typeof(Input).GetField("Aim"))))
        {
            cursor.GotoNext(MoveType.After, instr => instr.MatchCall<Vector2>("op_Inequality"));
            cursor.EmitDup();
            cursor.EmitNot(); // Input.Aim != Zero  ->  Input.Aim == Zero
            cursor.EmitStloc(zeroAim); // Input.Aim == Zero

            cursor.EmitDelegate(SuperdashesAffected);

            cursor.EmitDup();
            cursor.EmitStloc(enabled);

            cursor.EmitOr(); // Input.Aim != Zero && SuperdashesAffected

            if (cursor.TryGotoNextBestFit(MoveType.Before,
                instr => instr.MatchBrfalse(out ILLabel l),
                instr => instr.MatchLdcI4(1),
                instr => instr.MatchCall(typeof(Input).GetMethod("GetAimVector", BindingFlags.Static | BindingFlags.Public))))
            {
                cursor.GotoNext(MoveType.After, instr => instr.MatchBrfalse(out ILLabel l));
                int aimCheckStartIndex = cursor.Index;


                cursor.GotoNext(MoveType.After, instr => instr.MatchLdcR4(0.99f), instr => instr.MatchBgeUn(out ILLabel l));
                cursor.GotoNext(MoveType.Before, instr => instr.MatchLdarg0(), instr => instr.MatchLdarg0(), instr => instr.MatchLdfld<Player>("Speed"));
                ILLabel skipCheck = il.DefineLabel();
                cursor.MarkLabel(skipCheck);


                cursor.Goto(aimCheckStartIndex);

                cursor.EmitLdloc(zeroAim);
                cursor.EmitLdloc(enabled);
                cursor.EmitAnd();
                cursor.EmitBrtrue(skipCheck);

                if (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCall(typeof(Calc).GetMethod("Angle", new Type[] { typeof(Vector2) }))))
                {
                    cursor.EmitLdarg0();
                    cursor.EmitLdloc(zeroAim);
                    cursor.EmitLdloc(enabled);
                    cursor.EmitAnd();
                    cursor.EmitDelegate(ModifySuperdashTarget);
                }
            }
        }

        static bool SuperdashesAffected() => Enabled && GetSetting<bool>(AffectSuperdashes);
        static Vector2 ModifySuperdashTarget(Vector2 orig, Player player, bool enabled) => enabled ? SnapAim(player.Speed) : orig;
    }
    
    static Vector2 SnapAim(Vector2 aim)
    {
        float num = aim.Angle();
        int num2 = num < 0f ? 1 : 0;
        float num3 = MathF.PI / 8f - num2 * ((float)Math.PI / 36f);

        Vector2 result;
        if (Calc.AbsAngleDiff(num, 0f) < num3) result = new Vector2(1f, 0f);
        else if (Calc.AbsAngleDiff(num, MathF.PI) < num3) result = new Vector2(-1f, 0f);
        else if (Calc.AbsAngleDiff(num, -MathF.PI / 2f) < num3) result = new Vector2(0f, -1f);
        else if (Calc.AbsAngleDiff(num, MathF.PI / 2f) < num3) result = new Vector2(0f, 1f);
        
        else result = new Vector2(Math.Sign(aim.X), Math.Sign(aim.Y)).SafeNormalize();

        return result;
    }
}