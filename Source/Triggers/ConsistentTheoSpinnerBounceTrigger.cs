using System;
using System.Linq;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.LeniencyHelper.Components;

namespace Celeste.Mod.LeniencyHelper.Triggers
{
    [CustomEntity("LeniencyHelper/ConsistentTheoSpinnerBounceTrigger")]
    public class ConsistentTheoSpinnerBounceTrigger : GenericTrigger
    {
        private BounceDirections bounceDir;
        public ConsistentTheoSpinnerBounceTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            bounceDir = data.Enum("BounceDirection", BounceDirections.All);
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
        public static void LoadHooks()
        {
            On.Celeste.Holdable.Update += LoadSpinners;
        }
        public static void UnloadHooks()
        {
            On.Celeste.Holdable.Update -= LoadSpinners;
        }
        public enum BounceDirections
        {
            None,
            Left,
            Right,
            All
        }

        private static void LoadSpinners(On.Celeste.Holdable.orig_Update orig, Holdable self)
        {
            SpinnerBounceComponent thisComponent = self.Entity.Get<SpinnerBounceComponent>();
            if (thisComponent is null || !thisComponent.enabled)
            {
                orig(self);
                return;
            }

            foreach (Entity entity in self.Entity.Scene.Entities)
            {
                if (entity.GetType().Name.ToLower().Contains("spinner")
                    && !entity.GetType().Name.ToLower().Contains("controller")
                    && self.OnHitSpinner is not null)
                {
                    if ((entity.Position - self.Entity.Position).Length() <= 32f)
                    {
                        entity.Collidable = true;
                    }
                }
            }

            orig(self);
        }
    }
}