using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/ExtendDashAttackOnPickup")]
public class ExtendDashAttackOnPickupController : GenericTweakController
{
    public ExtendDashAttackOnPickupController(EntityData data, Vector2 offset) : base(data, offset, "ExtendDashAttackOnPickup")
    {

    }
}