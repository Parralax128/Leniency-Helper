using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/InstantClimbHop")]
public class InstantClimbHopTrigger : GenericTweakTrigger
{
    public InstantClimbHopTrigger(EntityData data, Vector2 offset, EntityID id) : base(data, offset)
    {
        tweakName = "InstantClimbHop";
    }
}