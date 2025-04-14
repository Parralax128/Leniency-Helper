using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/InstantAcceleratedJumps")]
public class InstantAcceleratedJumpsTrigger : GenericTweakTrigger
{
    public InstantAcceleratedJumpsTrigger(EntityData data, Vector2 offset) : base(data, offset, "InstantAcceleratedJumps")
    {

    }
}