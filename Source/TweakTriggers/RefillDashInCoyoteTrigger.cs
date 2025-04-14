using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/RefillDashInCoyote")]
public class RefillDashInCoyoteTrigger : GenericTweakTrigger
{
    public RefillDashInCoyoteTrigger(EntityData data, Vector2 offset) : base(data, offset, "RefillDashInCoyote")
    {

    }
}