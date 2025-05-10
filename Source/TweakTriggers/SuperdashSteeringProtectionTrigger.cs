using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/SuperdashSteeringProtection")]
public class SuperdashSteeringProtectionTrigger : GenericTweakTrigger
{
    public SuperdashSteeringProtectionTrigger(EntityData data, Vector2 offset) : base(data, offset, "SuperdashSteeringProtection")
    {

    }
}