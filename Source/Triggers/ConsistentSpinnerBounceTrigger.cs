using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.LeniencyHelper.Components;
using Celeste.Mod.LeniencyHelper.Module;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[Tracked]
[CustomEntity("LeniencyHelper/ConsistentTheoSpinnerBounceTrigger")]
public class ConsistentSpinnerBounceTrigger : GenericTrigger
{
    [OnLoad]
    public static void LoadHooks()
    {
        Everest.Events.Level.OnAfterUpdate += LoadSpinners;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        Everest.Events.Level.OnAfterUpdate -= LoadSpinners;
    }

    static readonly Dictionary<Type, bool> IsSpinnerType = new();

    static bool CheckSpinnerType(Type type)
    {
        string name = type.Name.ToLower();
        return name.Contains("spinner") && !name.Contains("controller");
    }
    private static void LoadSpinners(Level level)
    {
        if (!level.Tracker.Entities.TryGetValue(typeof(ConsistentSpinnerBounceTrigger), out List<Entity> triggers)
            || triggers == null || triggers.Count == 0)
        { 
            return; 
        }

        foreach(Entity entity in level)
        {
            Type type = entity.GetType();
            if(!IsSpinnerType.ContainsKey(type))
            {
                IsSpinnerType.Add(type, CheckSpinnerType(type));
            }

            if (IsSpinnerType[type] && entity.Collider != null)
            {
                if (entity.CollideFirst<ConsistentSpinnerBounceTrigger>() is { } trigger
                    && trigger != null && trigger.forceLoadSpinners)
                {
                    entity.Collidable = true;
                }
            }
        }
    }


    public enum BounceDirections
    {
        None,
        Left,
        Right,
        All
    }

    private BounceDirections bounceDir;
    private bool forceLoadSpinners;
    public ConsistentSpinnerBounceTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        bounceDir = data.Enum("BounceDirection", BounceDirections.All);
        forceLoadSpinners = data.Bool("ForceLoadSpinners", true);
    }

    public override void Update()
    {
        foreach (Holdable hold in Scene.Tracker.GetComponents<Holdable>())
        {
            SpinnerBounceComponent component = null;

            //if has component but hasnt "collide with me" value in dict
            if (hold.Entity.Components.Get<SpinnerBounceComponent>() is SpinnerBounceComponent sbc)
            {
                if (!sbc.collidingWith.Keys.Contains(this))
                    sbc.collidingWith.Add(this, false);

                component = sbc;
            }

            if (hold.Entity.CollideCheck(this))
            {

                //if holdable collided with trigger but component doesnt exist
                if (hold.Entity.Components.Get<SpinnerBounceComponent>() is null)
                {
                    hold.Entity.Add(new SpinnerBounceComponent(enabled, bounceDir));
                    component = hold.Entity.Components.Get<SpinnerBounceComponent>();
                }

                if (!component.collidingWith.Keys.Contains(this))//adding "collide with me" to dictonary if hasnt
                    component.collidingWith.Add(this, false);

                bool entered = false;
                if (!component.collidingWith[this]) entered = true;

                if (GetFlagActive())
                {
                    if (entered && revertOnLeave) component.SaveSettings();

                    component.enabled = enabled; //applying new settings if trigger is active and holdable is inside
                    component.direction = bounceDir;

                    if (oneUse) RemoveSelf();
                }
            }
            else
            {
                if (component is not null && component.collidingWith[this] && revertOnLeave) //if holdable left trigger
                {
                    component.UndoSettings();
                }
            }


            if (component is not null)
                component.collidingWith[this] = hold.Entity.CollideCheck(this);
        }

        Collidable = GetFlagActive();
    }
}