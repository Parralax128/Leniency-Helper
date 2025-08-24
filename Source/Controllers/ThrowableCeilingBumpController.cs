﻿using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.LeniencyHelper.Components;

namespace Celeste.Mod.LeniencyHelper.Controllers;

[CustomEntity("LeniencyHelper/Controllers/ThrowableCeilingBumpController")]
public class ThrowableCeilingBumpController : GenericController
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Monocle.Scene.BeforeUpdate += BeforeUpdate;
        On.Monocle.Scene.AfterUpdate += AfterUpdate;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        On.Monocle.Scene.BeforeUpdate -= BeforeUpdate;
        On.Monocle.Scene.AfterUpdate -= AfterUpdate;
    }

    private static void BeforeUpdate(On.Monocle.Scene.orig_BeforeUpdate orig, Scene self)
    {
        orig(self);

        if (self.Tracker.Components.ContainsKey(typeof(UnceilingBumpComponent))
            || self.Tracker.GetComponents<UnceilingBumpComponent>() == null) return;
        foreach(UnceilingBumpComponent bumpComponent in self.Tracker.GetComponents<UnceilingBumpComponent>())
        {
            bumpComponent.BeforeUpdate();
        }
    }
    private static void AfterUpdate(On.Monocle.Scene.orig_AfterUpdate orig, Scene self)
    {
        orig(self);
        foreach (UnceilingBumpComponent bumpComponent in self.Tracker.GetComponents<UnceilingBumpComponent>())
        {
            bumpComponent.AfterUpdate();
        }
    }

    private bool disableGroundFriction;
    private string whiteList;

    public ThrowableCeilingBumpController(EntityData data, Vector2 offset) : base(data, offset, false) 
    {
        disableGroundFriction = data.Bool("DisableGroundFriction", false);
        whiteList = data.String("WhiteList", "Glider, TheoCrystal");
    }

    public override void GetOldSettings() { }
    public override void Apply(bool fromFlag)
    {
        foreach(Holdable holdComponent in Scene.Tracker.GetComponents<Holdable>())
        {
            if (this.whiteList != "*" && !this.whiteList.Contains(holdComponent.Entity.GetType().Name)) continue;
            
            UnceilingBumpComponent bumpComponent = holdComponent.Entity.Get<UnceilingBumpComponent>();

            if (bumpComponent == null)
            {
                holdComponent.Entity.Add(new UnceilingBumpComponent(this.disableGroundFriction));
            }
            else
            {
                bumpComponent.SetGroundFriction(this.disableGroundFriction);
                bumpComponent.disableCeilingBump = true;
            }
        }
    }
    public override void Undo(bool fromFlag)
    {
        if (!fromFlag) return;

        foreach (UnceilingBumpComponent bumpComponent in Scene.Tracker.GetComponents<UnceilingBumpComponent>())
        {
            bumpComponent.OnStopFlag();
        }
    }
}
