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
            LeniencyHelperModule.Log($"type \"{entity.GetType().Name}\" does not contain \"Speed\" field");
            RemoveSelf();
        }

        if (entity is not Actor)
        {
            LeniencyHelperModule.Log($"component attached not to Actor!");
            RemoveSelf();
        }
    }

    public void BeforeUpdate()
    {
        savedSpeed = GetEntitySpeed();
        prevCollidedCeiling = Entity.CollideCheck<Solid>(Entity.Position - Vector2.UnitY); // gravity helper support required
    }
    private static void Log(object o) => LeniencyHelperModule.Log(o);
    public void AfterUpdate()
    {
        Vector2 currentEntitySpeed = GetEntitySpeed();
        Vector2 resultSpeed = currentEntitySpeed;

        if (wasOnGround && disableGroundFriction)
        {
            if (Math.Sign(savedSpeed.X) == Math.Sign(currentEntitySpeed.X) 
                && Math.Abs(savedSpeed.X) > Math.Abs(currentEntitySpeed.X))
            {
                resultSpeed.X = savedSpeed.X;
            }
        }
        if(prevCollidedCeiling && disableCeilingBump)
        {
            if(Math.Sign(currentEntitySpeed.Y) == -Math.Sign(savedSpeed.Y))
            {
                if (speedDecceleration > 0f) resultSpeed.Y = 0f; //prevent stucking in the ceiling trying to fly upwards
                else resultSpeed.Y = savedSpeed.Y - speedDecceleration;
            }
        }
        else if(!prevCollidedCeiling)
        {
            speedDecceleration = savedSpeed.Y - currentEntitySpeed.Y;
        }

        SetEntitySpeed(resultSpeed);

        wasOnGround = (Entity as Actor).OnGround();
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
