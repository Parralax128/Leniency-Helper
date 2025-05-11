using Monocle;
using Microsoft.Xna.Framework;

using System;
using System.Reflection;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Components;

[Tracked(true)]
public class UnceilingBumpComponent : Component
{
    public bool disableCeilingBump;
    public bool disableGroundFriction;

    private FieldInfo? speedField;
    public Vector2 savedSpeed;
    public float speedDecceleration;
    public bool wasOnGround;

    public bool prevCollidedCeiling;

    public UnceilingBumpComponent(bool disableGroundFriction) : base(true, true)
    {
        disableCeilingBump = true;
        this.disableGroundFriction = disableGroundFriction;
    }
    public override void Added(Entity entity)
    {
        base.Added(entity);

        speedField = entity.GetType().GetField("Speed");
        if (speedField == null) speedField = entity.GetType().GetField("Speed", BindingFlags.NonPublic);

        if (speedField == null || speedField.FieldType != typeof(Vector2))
        {
            LeniencyHelperModule.Log($"{entity.GetType().Name} does not contain \"Speed\" field");
            RemoveSelf();
        }

        if (entity is not Actor)
        {
            LeniencyHelperModule.Log($"component attached to not Actor!");
            RemoveSelf();
        }
    }

    public void BeforeUpdate()
    {
        wasOnGround = (Entity as Actor).OnGround();
        savedSpeed = GetEntitySpeed();
        prevCollidedCeiling = Entity.CollideCheck<Solid>(Entity.Position - Vector2.UnitY); // gravity helper support required
    }
    private static void Log(object o) => LeniencyHelperModule.Log(o);
    public void AfterUpdate()
    {
        bool shouldChange = false;
        Vector2 currentEntitySpeed = GetEntitySpeed();
        Vector2 resultSpeed = currentEntitySpeed;

        if (disableGroundFriction)
        {
            if ((Entity as Actor).OnGround() && Math.Sign(savedSpeed.Y) == -Math.Sign(currentEntitySpeed.Y))
            {
                resultSpeed.Y = 0f; //removing bounces from heavy throwables
                shouldChange = true;
            }

            if(wasOnGround)
            {
                if (Math.Sign(savedSpeed.X) == Math.Sign(currentEntitySpeed.X)
                && Math.Abs(savedSpeed.X) > Math.Abs(currentEntitySpeed.X))
                {
                    resultSpeed.X = savedSpeed.X; //disabling ground friction
                    shouldChange = true;
                }
            }
        }
        if(Entity.CollideCheck<Solid>(Entity.Position - Vector2.UnitY) && disableCeilingBump)
        {
            if(Math.Sign(currentEntitySpeed.Y) * Math.Sign(savedSpeed.Y) == -1)
            {
                resultSpeed.Y = savedSpeed.Y + speedDecceleration; //deccelerating instead of ceiling-bumping
                shouldChange = true;
            }
        }
        else if(!prevCollidedCeiling)
        {
            speedDecceleration = Math.Max(speedDecceleration, currentEntitySpeed.Y - savedSpeed.Y);
        }

        if(shouldChange)
        {
            SetEntitySpeed(resultSpeed);
        }
    }

    public void SetGroundFriction(bool value) =>
        disableGroundFriction = disableGroundFriction || value;

    public void OnStopFlag()
    {
        disableCeilingBump = false;
        disableGroundFriction = false;
    }
    public Vector2 GetEntitySpeed() => (Vector2)speedField.GetValue(Entity);
    public void SetEntitySpeed(Vector2 speed) => speedField.SetValue(Entity, speed);
}
