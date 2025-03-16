using System;
using Microsoft.Xna.Framework;
using Monocle;
using static Celeste.Mod.LeniencyHelper.Triggers.ConsistentTheoSpinnerBounceTrigger;
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

    public Dictionary<ConsistentTheoSpinnerBounceTrigger, bool> collidingWith = 
        new Dictionary<ConsistentTheoSpinnerBounceTrigger, bool>();

    public Holdable holdComponent
    {
        get
        {
            foreach(Component item in this.Entity)
            {
                if (item is Holdable hold) return hold;
            }
            return null;
        }
    }
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
    public override void Update()
    {
        var orig = holdComponent.OnHitSpinner;
        if (enabled)
        {
            holdComponent.OnHitSpinner = (spinner) =>
            {
                Vector2 saveSpeed = holdComponent.GetSpeed();
                orig(spinner);
                SetSpeed(saveSpeed);
            };
        }
        else
        {
            holdComponent.OnHitSeeker = orig;
        }
    }
    private void SetEntitySpeed(Entity entity, Vector2 speed)
    {
        FieldInfo speedField = entity.GetType().GetField("Speed");
        if (speedField is not null)
        {
            if (speedField.FieldType.ToString().ToLower() == "microsoft.xna.framework.vector2")
            {
                speedField.SetValue(entity, speed);
                return;
            }
        }

        PropertyInfo speedProperty = entity.GetType().GetProperty("Speed");
        if (speedProperty is not null)
        {
            if (speedProperty.PropertyType.ToString().ToLower() == "microsoft.xna.framework.vector2"
                && speedProperty.GetSetMethod() is not null)
            {
                speedProperty.SetValue(entity, speed);
            }
        }

    }
    public void SetSpeed(Vector2 saveSpeed)
    {
        if (holdComponent.GetSpeed().LengthSquared() > saveSpeed.LengthSquared() && Math.Abs(saveSpeed.X) < 0.01f)
        {
            switch (direction)
            {
                case BounceDirections.None:
                    holdComponent.SetSpeed(saveSpeed);

                    if (holdComponent.GetSpeed().Length() > 0.1f)
                        SetEntitySpeed(holdComponent.Entity, Vector2.Zero);

                    break;

                case BounceDirections.Left:
                    if (holdComponent.GetSpeed().X > 0f)
                    {
                        holdComponent.SetSpeed(new Vector2(-holdComponent.GetSpeed().X, holdComponent.GetSpeed().Y));
                        
                        if (holdComponent.GetSpeed().X > 0f)
                            SetEntitySpeed(holdComponent.Entity, new Vector2(-holdComponent.GetSpeed().X, holdComponent.GetSpeed().Y));
                    }
                    break;

                case BounceDirections.Right:
                    if (holdComponent.GetSpeed().X < 0f)
                    {
                        holdComponent.SetSpeed(new Vector2(-holdComponent.GetSpeed().X, holdComponent.GetSpeed().Y));

                        if (holdComponent.GetSpeed().X < 0f)
                            SetEntitySpeed(holdComponent.Entity, new Vector2(-holdComponent.GetSpeed().X, holdComponent.GetSpeed().Y));
                    }
                    break;

                case BounceDirections.All:
                    break;
            }
        }
    }
}