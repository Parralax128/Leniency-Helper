using System;
using Microsoft.Xna.Framework;
using Monocle;
using static Celeste.Mod.LeniencyHelper.Triggers.ConsistentSpinnerBounceTrigger;
using System.Reflection;
using System.Collections.Generic;
using Celeste.Mod.LeniencyHelper.Triggers;

namespace Celeste.Mod.LeniencyHelper.Components;

public class SpinnerBounceComponent : Component
{
    public bool enabled;
    public BounceDirections direction;

    public bool wasEnabled;
    public BounceDirections wasDirection;

    public Dictionary<ConsistentSpinnerBounceTrigger, bool> collidingWith = new();

    public Holdable HoldComponent => Entity?.Get<Holdable>();
    public SpinnerBounceComponent(bool enable, BounceDirections dir) : base(true, false) 
    {
        enabled = enable;
        direction = dir;
    }
    public void UndoSettings()
    {
        enabled = wasEnabled;
        direction = wasDirection;
    }
    public void SaveSettings()
    {
        wasEnabled = enabled;
        wasDirection = direction;
    }
    public override void Added(Entity to)
    {
        base.Added(to);

        Action<Entity> orig = HoldComponent.OnHitSpinner;

        HoldComponent.OnHitSpinner = (spinner) =>
        {
            Vector2 savedSpeed = HoldComponent.GetSpeed();
            orig?.Invoke(spinner);
            if(enabled) SetSpeed(savedSpeed);
        };
    }
    private void SetEntitySpeed(Entity entity, Vector2 speed)
    {
        FieldInfo speedField = entity.GetType().GetField("Speed");
        if (speedField != null)
        {
            if (speedField.FieldType == typeof(Vector2))
            {
                speedField.SetValue(entity, speed);
                return;
            }
        }

        PropertyInfo speedProperty = entity.GetType().GetProperty("Speed");
        if (speedProperty != null)
        {
            if (speedProperty.PropertyType == typeof(Vector2) && speedProperty.GetSetMethod() != null)
            {
                speedProperty.SetValue(entity, speed);
            }
        }
    }
    public void SetSpeed(Vector2 savedSpeed)
    {
        Vector2 speed = HoldComponent.GetSpeed();

        if (speed.LengthSquared() <= savedSpeed.LengthSquared() || Math.Abs(savedSpeed.X) >= 0.01f)
            return;
        
        switch (direction)
        {
            case BounceDirections.None:
                HoldComponent.SetSpeed(savedSpeed);

                if (HoldComponent.GetSpeed().Length() > 0.1f)
                    SetEntitySpeed(HoldComponent.Entity, Vector2.Zero);

                break;

            case BounceDirections.Left:
                if (speed.X > 0f)
                {
                    HoldComponent.SetSpeed(new Vector2(-speed.X, speed.Y));
                        
                    if ((speed = HoldComponent.GetSpeed()).X > 0f)
                        SetEntitySpeed(HoldComponent.Entity, new Vector2(-speed.X, speed.Y));
                }
                break;

            case BounceDirections.Right:
                if (speed.X < 0f)
                {
                    HoldComponent.SetSpeed(new Vector2(-speed.X, speed.Y));

                    if ((speed = HoldComponent.GetSpeed()).X < 0f)
                        SetEntitySpeed(HoldComponent.Entity, new Vector2(-speed.X, speed.Y));
                }
                break;

            case BounceDirections.All:
                break;
        }
        
    }
}