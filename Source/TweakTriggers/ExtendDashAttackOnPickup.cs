using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/ExtendDashAttackOnPickup")]
public class ExtendDashAttackOnPickupTrigger : GenericTweakTrigger
{
    public ExtendDashAttackOnPickupTrigger(EntityData data, Vector2 offset) : base(data, offset, "ExtendDashAttackOnPickup")
    {

    }
}