using Celeste.Mod.LeniencyHelper.Triggers;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Reflection;
using static Celeste.Mod.LeniencyHelper.Triggers.SpinnerBounceTrigger;

namespace Celeste.Mod.LeniencyHelper.Components;

class SpinnerBounceComponent : Component
{
    public bool Enabled;
    public BounceDirections Direction;

    bool savedEnabled;
    BounceDirections savedDirection;

    private enum SpeedAccessor
    { 
        Field, 
        Property, 
        None 
    };
    SpeedAccessor accessor = SpeedAccessor.None;

    FieldInfo speedField;
    PropertyInfo speedProperty;

    public bool this[SpinnerBounceTrigger trigger]
    {
        get => triggersCollided.Contains(trigger);
        set
        {
            if(value) triggersCollided.Add(trigger);
            else triggersCollided.Remove(trigger);
        }
    }

    HashSet<SpinnerBounceTrigger> triggersCollided = new();

    public Holdable HoldComponent;
    public SpinnerBounceComponent(bool enable, BounceDirections dir) : base(true, false) 
    {
        Enabled = enable;
        Direction = dir;
    }
    public override void Added(Entity entity)
    {
        base.Added(entity);
        HoldComponent = entity.Get<Holdable>();

        Action<Entity> orig = HoldComponent.OnHitSpinner;

        HoldComponent.OnHitSpinner = (spinner) =>
        {
            if (Enabled)
            {
                Vector2 savedSpeed = HoldComponent.GetSpeed();
                orig?.Invoke(spinner);
                ProcessBounceSpeed(savedSpeed);
            }
            else orig?.Invoke(spinner);
        };



        if (entity.GetType().GetField("Speed") is FieldInfo field && field.FieldType == typeof(Vector2))
        {
            speedField = field;
            accessor = SpeedAccessor.Field;
            return;
        }

        
        if (entity.GetType().GetProperty("Speed") is PropertyInfo property
            && property.PropertyType == typeof(Vector2) && property.GetSetMethod() != null)
        {
            speedProperty = property;
            accessor = SpeedAccessor.Property;
        }
    }

    public void SaveSettings()
    {
        savedEnabled = Enabled;
        savedDirection = Direction;
    }
    public void UndoSettings()
    {
        Enabled = savedEnabled;
        Direction = savedDirection;
    }

    
    void SetEntitySpeed(Entity entity, Vector2 speed)
    {
        if(accessor == SpeedAccessor.Field)
        {
            speedField.SetValue(entity, speed);
            return;
        }

        if(accessor == SpeedAccessor.Property)
            speedProperty.SetValue(entity, speed);
    }
    public void ProcessBounceSpeed(Vector2 savedSpeed)
    {
        Vector2 currentSpeed = HoldComponent.GetSpeed();
        if (currentSpeed.LengthSquared() <= savedSpeed.LengthSquared()
            || Math.Abs(savedSpeed.X) >= 0.01f
            || Direction == BounceDirections.All) 
        { 
            return; 
        }

        if(HoldComponent.SpeedSetter != null)
        {
            HoldComponent.SetSpeed(new Vector2(
                Math.Abs(currentSpeed.X) * (int)Direction,
                currentSpeed.Y * Math.Abs((int)Direction)));
        }
        else
        {
            SetEntitySpeed(HoldComponent.Entity, new Vector2(
                Math.Abs(currentSpeed.X) * (int)Direction,
                currentSpeed.Y * Math.Abs((int)Direction)));
        }
        return;

        switch (Direction)
        {

            case BounceDirections.None:
                HoldComponent.SetSpeed(savedSpeed);

                if (HoldComponent.GetSpeed().LengthSquared() > 0.1f)
                    SetEntitySpeed(HoldComponent.Entity, Vector2.Zero);
                return;


            case BounceDirections.Left when HoldComponent.GetSpeed().X > 0f:
                HoldComponent.SetSpeed(new Vector2(-HoldComponent.GetSpeed().X, HoldComponent.GetSpeed().Y));

                if (HoldComponent.GetSpeed().X > 0f)
                    SetEntitySpeed(HoldComponent.Entity, new Vector2(-HoldComponent.GetSpeed().X, HoldComponent.GetSpeed().Y));
                return;


            case BounceDirections.Right when HoldComponent.GetSpeed().X < 0f:
                HoldComponent.SetSpeed(new Vector2(-HoldComponent.GetSpeed().X, HoldComponent.GetSpeed().Y));

                if (HoldComponent.GetSpeed().X < 0f)
                    SetEntitySpeed(HoldComponent.Entity, new Vector2(-HoldComponent.GetSpeed().X, HoldComponent.GetSpeed().Y));

                return;
        }
    }
}