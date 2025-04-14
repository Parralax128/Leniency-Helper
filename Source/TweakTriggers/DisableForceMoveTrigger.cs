using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DisableForceMove")]
public class DisableForceMoveTrigger : GenericTweakTrigger
{
    public DisableForceMoveTrigger(EntityData data, Vector2 offset) : base(data, offset, "DisableForceMove")
    {

    }
}
