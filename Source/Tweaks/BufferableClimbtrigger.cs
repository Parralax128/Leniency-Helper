using Monocle;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using System;
using MonoMod.Cil;
using static Celeste.Mod.Helpers.ILCursorExtensions;
using Celeste.Mod.LeniencyHelper.Components;
using VivHelper.Entities;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class BufferableClimbtrigger
{
    public static void LoadHooks()
    {
        On.Celeste.Player.ClimbJump += ClimbTriggerOnClimbJump;
        IL.Celeste.Player.WallJumpCheck += GetWJCheckSolid;

        IL.Celeste.Player.NormalUpdate += ClimbTriggerOnNormalUpdate;
        IL.Celeste.Player.SwimUpdate += ClimbTriggerOnSwimUpdate;

        On.Celeste.Player.IsRiding_Solid += ForceRideSolid;
        On.Celeste.Solid.GetPlayerClimbing += ForceClimbSolid;
        On.Celeste.BounceBlock.WindUpPlayerCheck += ForceCoreBlockTrigger;

        
        IL.Celeste.Solid.MoveHExact += DontMovePlayer;
        IL.Celeste.Solid.MoveVExact += DontMovePlayer;
    }
    public static void UnloadHooks()
    {
        On.Celeste.Player.ClimbJump -= ClimbTriggerOnClimbJump;
        IL.Celeste.Player.WallJumpCheck -= GetWJCheckSolid;

        IL.Celeste.Player.NormalUpdate -= ClimbTriggerOnNormalUpdate;
        IL.Celeste.Player.SwimUpdate -= ClimbTriggerOnSwimUpdate;

        On.Celeste.Player.IsRiding_Solid -= ForceRideSolid;
        On.Celeste.Solid.GetPlayerClimbing -= ForceClimbSolid;
        On.Celeste.BounceBlock.WindUpPlayerCheck -= ForceCoreBlockTrigger;

        
        IL.Celeste.Solid.MoveHExact -= DontMovePlayer;
        IL.Celeste.Solid.MoveVExact -= DontMovePlayer;
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

    public static void DelegateCollideCheck(Vector2 at, Player player)
    {
        if(player.CollideCheck<Solid>(at))
            LeniencyHelperModule.Session.climbSolid = player.CollideFirst<Solid>(at);
    }
    private static void ClimbTriggerOnNormalUpdate(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        cursor.GotoNext(MoveType.Before, instr => instr.MatchCallvirt<Player>("ClimbTrigger"));
        cursor.GotoPrev(MoveType.Before, instr => instr.MatchLdarg0(),
            instr => instr.MatchLdflda<Player>("Speed"),
            instr => instr.MatchLdfld<Vector2>("Y"));

        cursor.Index++;
        cursor.EmitLdcI4(0);
        cursor.EmitDelegate(ForceClimbTrigger);
        cursor.EmitLdarg0();
    }
    private static void ClimbTriggerOnSwimUpdate(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        cursor.GotoNext(MoveType.Before, instr => instr.MatchCallvirt<Player>("ClimbTrigger"));
        cursor.GotoPrev(MoveType.Before, instr => instr.MatchLdarg0(),
            instr => instr.MatchLdflda<Player>("Speed"),
            instr => instr.MatchLdfld<Vector2>("Y"));

        cursor.Index++;
        cursor.EmitLdcI4(0);
        cursor.EmitDelegate(ForceClimbTrigger);
        cursor.EmitLdarg0();
    }
    private static void ForceClimbTrigger(Player player, bool swim)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["BufferableClimbtrigger"] ||
            (bool)LeniencyHelperModule.Settings.GetSetting("BufferableClimbtrigger", "onlyOnClimbjumps")) return;
        if(!swim)
        {
            if (Math.Sign(player.Speed.X) != 0 - player.Facing
                && player.ClimbCheck((int)player.Facing))
            {
                var s = LeniencyHelperModule.Session;
                if(s.climbSolid is not null && s.climbSolid.Get<ForceRideComponent>() is null)
                    s.climbSolid.Add(new ForceRideComponent());
            }
        }
        else
        {
            if(!player.SwimUnderwaterCheck() && Input.GrabCheck &&
                !player.IsTired && player.CanUnDuck
                && Math.Sign(player.Speed.X) != 0 - player.Facing
                && player.ClimbCheck((int)player.Facing))
            {
                var s = LeniencyHelperModule.Session;
                if (s.climbSolid is not null && s.climbSolid.Get<ForceRideComponent>() is null)
                    s.climbSolid.Add(new ForceRideComponent());
            }
        }
    }
    private static void GetWJCheckSolid(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        ILLabel gotoLabel = il.DefineLabel();

        if (cursor.TryGotoNextBestFit(MoveType.Before,
            instr => instr.MatchCall<Vector2>("op_Addition"),
            instr => instr.MatchCall<Entity>("CollideCheck"),
            instr => instr.MatchRet()))
        {
            if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCall<Entity>("CollideCheck")))
            {
                cursor.EmitDup();
                cursor.EmitLdarg0();
                cursor.EmitDelegate(DelegateCollideCheck);
            }   
        }
    }

    private static bool useOrigCheck = false;
    private static bool ForceRideSolid(On.Celeste.Player.orig_IsRiding_Solid orig, Player self, Solid solid)
    {
        return ((!useOrigCheck && ConsumeComponent(solid)) || orig(self, solid));
    }
    private static void ClimbTriggerOnClimbJump(On.Celeste.Player.orig_ClimbJump orig, Player self)
    {
        orig(self);
        var s = LeniencyHelperModule.Session;

        if (s.TweaksEnabled["BufferableClimbtrigger"] && s.climbSolid is not null && s.climbSolid.Get<ForceRideComponent>() is null)
        {
            s.climbSolid.Add(new ForceRideComponent());
        }
    }

    private static Player ForceCoreBlockTrigger(On.Celeste.BounceBlock.orig_WindUpPlayerCheck orig, BounceBlock self)
    {
        if (ConsumeComponent(self)) return self.Scene.Tracker.GetEntity<Player>();
        else return orig(self);
    }
    private static Player ForceClimbSolid(On.Celeste.Solid.orig_GetPlayerClimbing orig, Solid self)
    {
        if (ConsumeComponent(self)) return self.Scene.Tracker.GetEntity<Player>();
        else return orig(self);
    }

    private static Player ForceCustomCoreblockTrigger(Func<ReskinnableBounceBlock, Player> orig, ReskinnableBounceBlock self)
    {
        return orig(self);
    }


    private static bool ConsumeComponent(Entity entity)
    {
        if (LeniencyHelperModule.Session.TweaksEnabled["BufferableClimbtrigger"] && entity.Components.Get<ForceRideComponent>() is not null)
        {
            entity.Remove(entity.Components.Get<ForceRideComponent>());
            return true;
        }
        return false;
    }
}