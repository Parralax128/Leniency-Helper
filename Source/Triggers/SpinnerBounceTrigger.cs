using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.LeniencyHelper.Components;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[Tracked]
[CustomEntity("LeniencyHelper/ConsistentTheoSpinnerBounceTrigger")]
class SpinnerBounceTrigger : GenericTrigger
{
    

    [OnLoad]
    public static void LoadHooks()
    {
        Everest.Events.Level.OnAfterUpdate += ForceLoadSpinners;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        Everest.Events.Level.OnAfterUpdate -= ForceLoadSpinners;
    }

    static HashSet<Type> SpinnerTypes = new();
    static void ForceLoadSpinners(Level level)
    {
        foreach (KeyValuePair<Type, List<Entity>> pair in level.Tracker.Entities)
        {
            Type entityType = pair.Key;
            if (!SpinnerTypes.Contains(entityType))
            {
                if (entityType.Name.Contains("spinner")) SpinnerTypes.Add(entityType);
                else continue;
            }

            foreach (Entity spinner in pair.Value)
                if (spinner?.CollideFirst<SpinnerBounceTrigger>() is SpinnerBounceTrigger trigger && trigger.forceLoadSpinners)
                    spinner.Collidable = true;
        }
    }   
    
    public enum BounceDirections
    {
        None = 0,
        Left = -1,
        Right = 1,
        All = 1
    }

    BounceDirections bounceDir;
    bool forceLoadSpinners;
    public SpinnerBounceTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        bounceDir = data.Enum("BounceDirection", BounceDirections.All);
        forceLoadSpinners = data.Bool("ForceLoadSpinners", true);
    }
    
    public override void Update()
    {
        foreach (Entity holdable in Scene.Tracker.GetComponents<Holdable>().Select(h => h.Entity))
        {
            SpinnerBounceComponent component = null;

            if (holdable.CollideCheck(this))
            {
                component = holdable.Components.Get<SpinnerBounceComponent>();
                if (component == null) holdable.Add(component = new SpinnerBounceComponent(Enabled, bounceDir));

                // holdable entered trigger
                if (!component[this] && RevertOnLeave)
                    component.SaveSettings();

                // apply new settings
                component.Enabled = Enabled;
                component.Direction = bounceDir;

                if (OneUse) RemoveSelf();
            }
            else if (component?[this] == true && RevertOnLeave) // holdable left trigger
            { 
                component.UndoSettings(); 
            }

            if (component != null) component[this] = holdable.CollideCheck(this);
        }
    }
}