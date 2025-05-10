using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/SuperdashSteeringProtection")]
public class SuperdashSteeringProtectionController : GenericTweakController
{
    public SuperdashSteeringProtectionController(EntityData data, Vector2 offset) : base(data, offset, "SuperdashSteeringProtection")
    {

    }
}