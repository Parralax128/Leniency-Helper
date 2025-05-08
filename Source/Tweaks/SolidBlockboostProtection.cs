using System;
using Microsoft.Xna.Framework;
using Celeste.Mod.LeniencyHelper.Components;
using Monocle;
using MonoMod.Cil;
using Celeste.Mod.MaxHelpingHand.Entities;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class SolidBlockboostProtection : AbstractTweak
{
    private static Hook SidewaysAddedHook = null;
    private static Hook SidewaysOnMoveHook = null;
    private static ILHook SidewaysUpdate = null;

    #region normal platforms
    [OnLoad]
    public static void LoadHooks()
    {

        On.Celeste.Platform.Update += GainSavedBlockboost;
        On.Celeste.Platform.ctor += GiveComponentToPlatform;

        IL.Celeste.JumpThru.MoveHExact += ComponentOnMove;
        IL.Celeste.Solid.MoveHExact += ComponentOnMove;
        IL.Celeste.JumpThru.MoveVExact += ComponentOnMove;
        IL.Celeste.Solid.MoveVExact += ComponentOnMove;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Platform.Update -= GainSavedBlockboost;
        On.Celeste.Platform.ctor -= GiveComponentToPlatform;

        IL.Celeste.JumpThru.MoveHExact -= ComponentOnMove;
        IL.Celeste.Solid.MoveHExact -= ComponentOnMove;
        IL.Celeste.JumpThru.MoveVExact -= ComponentOnMove;
        IL.Celeste.Solid.MoveVExact -= ComponentOnMove;

        if(LeniencyHelperModule.ModLoaded("MaxHelpingHand"))
        {
            UnloadSidewaysHook();
        }
    }


    private static void GiveComponentToPlatform(On.Celeste.Platform.orig_ctor orig, Platform self, Vector2 pos, bool safe)
    {   
        orig(self, pos, safe);

        if (self is not SolidTiles) self.Add(new SolidLiftboostComponent());
    }
    private static void GainSavedBlockboost(On.Celeste.Platform.orig_Update orig, Platform self)
    {
        orig(self);

        if (!Enabled("SolidBlockboostProtection")) return;

        SolidLiftboostComponent component = self.Get<SolidLiftboostComponent>();
        if (component is null || component.boostSaveTimer <= 0f) return;

        if (self.Collidable && self.LiftSpeed.LengthSquared() <= 0.01f)
        {
            foreach (Actor actor in self.Scene.Tracker.GetEntities<Actor>())
            {
                if (!actor.AllowPushing) continue;

                bool saveCollidable = actor.Collidable;
                actor.Collidable = true;


                if ((self is JumpThru jt && actor.IsRiding(jt)) || (self is Solid solid && actor.IsRiding(solid)))
                {
                    actor.LiftSpeed = component.savedLiftSpeed;
                }
                else if (LeniencyHelperModule.CollideOnWJdist(self, actor, Math.Sign(component.savedLiftSpeed.X), null))
                {
                    actor.LiftSpeed = actor.LiftSpeed with { X = component.savedLiftSpeed.X };
                }
                else if (self.CollideCheck(actor, self.Position + Math.Sign(component.savedLiftSpeed.Y) * Vector2.UnitY))
                {
                    actor.LiftSpeed = actor.LiftSpeed with { Y = component.savedLiftSpeed.Y };
                }

                actor.Collidable = saveCollidable;
            }
        }

        component.boostSaveTimer -= Engine.DeltaTime;
    }
    
    private static void ComponentOnMove(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        
        while (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchRet()))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(MoveDelegate);
            cursor.Index++;
        }
    }
    private static void MoveDelegate(Entity self)
    {
        if (Enabled("SolidBlockboostProtection") && self is Platform p && p.LiftSpeed.LengthSquared() > 0.01f)
        {
            p.Get<SolidLiftboostComponent>().OnMove();
        }
    }
    #endregion


    #region SidewaysJumpthru
    public static void LoadSidewaysHook()
    {
        SidewaysAddedHook = new Hook(typeof(AttachedSidewaysJumpThru).GetMethod("Added"), SidewaysComponentOnAdded);
        SidewaysOnMoveHook = new Hook(typeof(SidewaysMovingPlatform).GetMethod("SidewaysJumpthruOnMove"), SidewaysOnMove);
        SidewaysUpdate = new ILHook(typeof(AttachedSidewaysJumpThru).GetMethod("Update"), ModifyUpdate);
    }
    public static void UnloadSidewaysHook()
    {
        SidewaysAddedHook?.Dispose();
        SidewaysOnMoveHook?.Dispose();
        SidewaysUpdate?.Dispose();
    }

    private static void SidewaysComponentOnAdded(Action<SidewaysJumpThru, Scene> orig, SidewaysJumpThru self, Scene scene)
    {
        orig(self, scene);
        self.Add(new SolidLiftboostComponent());
    }
    private static void SidewaysOnMove(Action<Entity, Solid, bool, Vector2> orig,
        Entity platform, Solid playerInteractingSolid, bool left, Vector2 move)
    {
        orig(platform, playerInteractingSolid, left, move);

        if(Enabled("SolidBlockboostProtection") && LeniencyHelperModule.ModLoaded("MaxHelpingHand") && (platform is AttachedSidewaysJumpThru))
        {
            platform.Get<SolidLiftboostComponent>()?.OnSidewaysMove(playerInteractingSolid.LiftSpeed);
        }
    }
    private static void ModifyUpdate(ILContext il)
    {        
        ILCursor c = new ILCursor(il);

        ILLabel skip = il.DefineLabel();


        c.TryGotoNext(MoveType.After, instr => instr.MatchCall<Entity>("Update"));

        c.EmitLdarg0();
        c.EmitIsinst(typeof(AttachedSidewaysJumpThru));
        c.EmitBrfalse(skip);

        c.EmitLdarg0();
        c.EmitIsinst(typeof(AttachedSidewaysJumpThru));
        c.EmitDup();
        c.EmitLdfld(typeof(AttachedSidewaysJumpThru).GetField("playerInteractingSolid", BindingFlags.NonPublic | BindingFlags.Instance));
        c.EmitDelegate(GainSidewaysBlockboost);

        c.MarkLabel(skip);
    }

    private static void GainSidewaysBlockboost(AttachedSidewaysJumpThru self, Solid interactSolid)
    {
        if (!Enabled("SolidBlockboostProtection")) return;

        SolidLiftboostComponent component = self.Get<SolidLiftboostComponent>();
        if (component is null || component.boostSaveTimer <= 0f) return;


        if (self.Collidable && interactSolid.LiftSpeed.LengthSquared() <= 0.01f)
        {
            foreach (Actor actor in self.Scene.Tracker.GetEntities<Actor>())
            {
                if (!actor.AllowPushing) continue;

                bool saveCollidable = actor.Collidable;
                actor.Collidable = true;

                if (LeniencyHelperModule.CollideOnWJdist(self, actor, Math.Sign(component.savedLiftSpeed.X),
                    self.Position + Vector2.UnitY * Math.Sign(component.savedLiftSpeed.Y)))
                {
                    actor.LiftSpeed = component.savedLiftSpeed;
                }
                else if (LeniencyHelperModule.CollideOnWJdist(self, actor, Math.Sign(component.savedLiftSpeed.X), null))
                {
                    actor.LiftSpeed = actor.LiftSpeed with { X = component.savedLiftSpeed.X };
                }
                else if (self.CollideCheck(actor, self.Position + Math.Sign(component.savedLiftSpeed.Y) * Vector2.UnitY))
                {
                    actor.LiftSpeed = actor.LiftSpeed with { Y = component.savedLiftSpeed.Y };
                }

                actor.Collidable = saveCollidable;
            }
        }
        component.boostSaveTimer -= Engine.DeltaTime;
    }

    #endregion
}