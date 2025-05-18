using Microsoft.Xna.Framework;
using System;
using MonoMod.Cil;
using static Celeste.Mod.Helpers.ILCursorExtensions;
using VivHelper.Entities;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class BufferableClimbtrigger : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.ClimbJump += ClimbTriggerOnClimbJump;
        On.Celeste.Player.DashUpdate += ClimbTriggerDuringDash;
        IL.Celeste.Player.NormalUpdate += ClimbTriggerOnFlyingUp;

        On.Celeste.Player.ClimbTrigger += GetDefaultDir;
        On.Celeste.Player.Update += ClearSafeClimbTrigger;

        On.Celeste.Player.IsRiding_Solid += ForceRideSolid;
        On.Celeste.Solid.GetPlayerClimbing += ForceClimbSolid;
        On.Celeste.BounceBlock.WindUpPlayerCheck += ForceCoreBlockTrigger;
        
        IL.Celeste.Solid.MoveHExact += DontMovePlayer;
        IL.Celeste.Solid.MoveVExact += DontMovePlayer;
    }
    private static Hook customWindUpHook;
    public static void LoadVivHelperHooks()
    {
        customWindUpHook = new Hook(typeof(ReskinnableBounceBlock).GetMethod("WindUpPlayerCheck",
            BindingFlags.NonPublic | BindingFlags.Instance), ForceCustomCoreblockTrigger);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.ClimbJump -= ClimbTriggerOnClimbJump;
        On.Celeste.Player.DashUpdate -= ClimbTriggerDuringDash;
        IL.Celeste.Player.NormalUpdate -= ClimbTriggerOnFlyingUp;

        On.Celeste.Player.Update -= ClearSafeClimbTrigger;

        On.Celeste.Player.IsRiding_Solid -= ForceRideSolid;
        On.Celeste.Solid.GetPlayerClimbing -= ForceClimbSolid;
        On.Celeste.Player.ClimbTrigger -= GetDefaultDir;
        On.Celeste.BounceBlock.WindUpPlayerCheck -= ForceCoreBlockTrigger;

        IL.Celeste.Solid.MoveHExact -= DontMovePlayer;
        IL.Celeste.Solid.MoveVExact -= DontMovePlayer;

        if (LeniencyHelperModule.ModLoaded("VivHelper")) UnloadVivHelperHooks();
    }
    public static void UnloadVivHelperHooks()
    {
        customWindUpHook.Dispose();
    }
    
    private static int safeClimbtriggerDir;
    private static bool useOrigCheck = false;


    private static void ClimbTriggerOnClimbJump(On.Celeste.Player.orig_ClimbJump orig, Player self)
    {
        orig(self);

        if (Enabled("BufferableClimbtrigger"))
        {
            self.ClimbTrigger((int)self.Facing);
        }
    }
    private static int ClimbTriggerDuringDash(On.Celeste.Player.orig_DashUpdate orig, Player self)
    {
        if (Enabled("BufferableClimbtrigger") && GetSetting<bool>("onDash"))
        {
            if (self.Holding == null && Math.Sign(self.Speed.X) != 0 - self.Facing && self.ClimbBoundsCheck((int)self.Facing)
                && Input.GrabCheck && !self.IsTired && !self.Ducking)
            {
                safeClimbtriggerDir = (int)self.Facing;
            }
        }
        return orig(self);
    }
    private static void ClimbTriggerOnFlyingUp(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        ILLabel skipClimbtriggerDelegate = il.DefineLabel();

        if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("ClimbTrigger")))
        {
            if (cursor.TryGotoPrevBestFit(MoveType.Before,
                instr => instr.MatchLdflda<Player>("Speed"),
                instr => instr.MatchLdfld<Vector2>("Y"),
                instr => instr.MatchLdcR4(0f),
                instr => instr.MatchBltUn(out ILLabel label)))
            {
                cursor.EmitDelegate(CheckEnabled);
                cursor.EmitBrfalse(skipClimbtriggerDelegate);

                cursor.EmitLdarg0();
                cursor.EmitDelegate(ClimbtriggerDelegate);

                cursor.MarkLabel(skipClimbtriggerDelegate);
                cursor.EmitLdarg0();
            }
            cursor.GotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("ClimbTrigger"));
        }
    }


    private static void ClearSafeClimbTrigger(On.Celeste.Player.orig_Update orig, Player self)
    {
        safeClimbtriggerDir = 0;
        orig(self);
    }
    private static void GetDefaultDir(On.Celeste.Player.orig_ClimbTrigger orig, Player self, int dir)
    {
        orig(self, dir);
        safeClimbtriggerDir = dir;
    }

    private static bool ForceRideSolid(On.Celeste.Player.orig_IsRiding_Solid orig, Player self, Solid solid)
    {
        return (!useOrigCheck && GetClimbTriggeringPlayer(solid, self) != null) || orig(self, solid);
    }
    private static Player ForceClimbSolid(On.Celeste.Solid.orig_GetPlayerClimbing orig, Solid self)
    {
        return orig(self) == null ? GetClimbTriggeringPlayer(self) : orig(self);
    }
    private static Player ForceCoreBlockTrigger(On.Celeste.BounceBlock.orig_WindUpPlayerCheck orig, BounceBlock self)
    {
        return orig(self) == null ? GetClimbTriggeringPlayer(self) : orig(self);
    }
    private static Player ForceCustomCoreblockTrigger(Func<ReskinnableBounceBlock, Player> orig, ReskinnableBounceBlock self)
    {
        return orig(self) == null ? GetClimbTriggeringPlayer(self) : orig(self);
    }

    private static Player GetClimbTriggeringPlayer(Solid solid, Player player = null)
    {
        if (!Enabled("BufferableClimbtrigger")) return null;

        if (player == null) player = solid.Scene.Tracker.GetNearestEntity<Player>(solid.Center);
        if (player == null || safeClimbtriggerDir == 0) return null;

        return LeniencyHelperModule.CollideOnWJdist(player, solid, safeClimbtriggerDir)? player : null;
    }


    private static void DontMovePlayer(ILContext il)
    {
        ILCursor c = new ILCursor(il);
        c.EmitLdcI4(1);
        c.EmitDelegate(SetUseOrig);

        while (c.TryGotoNext(MoveType.Before, i => i.MatchRet()))
        {
            c.EmitLdcI4(0);
            c.EmitDelegate(SetUseOrig);
            c.Index++;
        }
    }


    private static void SetUseOrig(bool value) { useOrigCheck = value; }
    
    private static void ClimbtriggerDelegate(Player player)
    {
        if (Math.Sign(player.Speed.X) != 0 - player.Facing)
            safeClimbtriggerDir = (int)player.Facing;
    }
    
    private static bool CheckEnabled(Player player)
    {
        return Enabled("BufferableClimbtrigger") && GetSetting<bool>("onNormalUpdate");
    }    
}