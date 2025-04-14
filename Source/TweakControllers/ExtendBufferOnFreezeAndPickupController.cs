using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/ExtendBufferOnFreezeAndPickup")]
public class ExtendBufferOnFreezeAndPickupController : GenericTweakController
{
    public ExtendBufferOnFreezeAndPickupController(EntityData data, Vector2 offset) : base(data, offset, "ExtendBufferOnFreezeAndPickup")
    {

    }
}