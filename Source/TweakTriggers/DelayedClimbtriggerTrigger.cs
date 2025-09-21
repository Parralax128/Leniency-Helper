using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DelayedClimbtrigger")]
public class DelayedClimbtriggerTrigger : GenericTweakTrigger
{
    public DelayedClimbtriggerTrigger(EntityData data, Vector2 offset) : base(data, offset, "DelayedClimbtrigger")
    {

    }
}