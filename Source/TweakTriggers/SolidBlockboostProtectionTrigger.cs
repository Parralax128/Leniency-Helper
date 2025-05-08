using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/SolidBlockboostProtection")]
public class SolidBlockboostProtectionTrigger : GenericTweakTrigger
{
    public SolidBlockboostProtectionTrigger(EntityData data, Vector2 offset) : base(data, offset, "SolidBlockboostProtection")
    {

    }
}