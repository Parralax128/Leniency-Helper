using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.LeniencyHelper.Components;
using Celeste.Mod.LeniencyHelper.Module;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[Tracked]
[CustomEntity("LeniencyHelper/ConsistentTheoSpinnerBounceTrigger")]
public class ConsistentTheoSpinnerBounceTrigger : GenericTrigger
{
    [OnLoad]
    public static void LoadHooks()
    {
        LeniencyHelperModule.OnUpdate += LoadSpinners;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        LeniencyHelperModule.OnUpdate -= LoadSpinners;
    }

    private static void LoadSpinners()
    {
        foreach (Entity spinner in Engine.Scene.Entities.ToList().FindAll(e =>
        e.GetType().Name.ToLower().Contains("spinner") && !e.GetType().Name.Contains("controller")))
        {
            var trigger = spinner.CollideFirst<ConsistentTheoSpinnerBounceTrigger>();

            if (trigger != null && trigger.forceLoadSpinners)
            {   
                spinner.Collidable = true;
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
    public ConsistentTheoSpinnerBounceTrigger(EntityData data, Vector2 offset) : base(data, offset)
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