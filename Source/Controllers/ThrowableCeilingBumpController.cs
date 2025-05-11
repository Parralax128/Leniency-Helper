using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Celeste.Mod.ShroomHelper.Entities;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Module;
using IL.Celeste.Mod.Registry.DecalRegistryHandlers;
using System.Runtime.CompilerServices;
using MonoMod;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System.Reflection;
using Celeste.Mod.LeniencyHelper.Components;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.Controllers;

[CustomEntity("LeniencyHelper/Controllers/ThrowableCeilingBumpController")]
public class ThrowableCeilingBumpController : GenericController
{
    #region hooks

    [OnLoad]
    public static void LoadHooks()
    {
        On.Monocle.Scene.BeforeUpdate += BeforeUpdate;
        On.Monocle.Scene.AfterUpdate += AfterUpdate;
    }

    private static void BeforeUpdate(On.Monocle.Scene.orig_BeforeUpdate orig, Scene self)
    {
        orig(self);
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

    private static void Log(object o) => LeniencyHelperModule.Log(o);

    #endregion

    private bool disableGroundFriction;
    string affectedThrowables;

    public ThrowableCeilingBumpController(EntityData data, Vector2 offset) : base(data, offset, false) 
    {
        disableGroundFriction = data.Bool("DisableGroundFriction", false);
        affectedThrowables = data.String("EntityList", "Glider, TheoCrystal");

        data.String("EntityList", "Glider, TheoCrystal");
    }

    public override void GetOldSettings() { }
    public override void Apply(bool fromFlag)
    {
        foreach(Holdable holdComponent in Scene.Tracker.GetComponents<Holdable>())
        {
            if (this.affectedThrowables != "*" && !this.affectedThrowables.Contains(holdComponent.Entity.GetType().Name)) continue;
            
            UnceilingBumpComponent? bumpComponent = holdComponent.Entity.Get<UnceilingBumpComponent>();

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

    private static List<string> SplitLineToList(string line)
    {
        List<string> result = new List<string>();

        string currentWord = "";
        for(int c=0; c<line.Length; c++)
        {
            if (line[c] == ' ') continue;

            else if (line[c] == ',' || c == line.Length - 1)
            {
                result.Add(currentWord);
                currentWord = "";
            }

            else currentWord += line[c];
        }
        return result;
    }
}
