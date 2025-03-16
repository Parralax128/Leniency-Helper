using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/BufferableClimbtrigger")]
public class BufferableClimbtriggerTrigger : GenericTweakTrigger
{
    public BufferableClimbtriggerTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "BufferableClimbtrigger";

        fullData = fullData.Append(new TriggerData(false, "onlyOnClimbjumps", "DisableInstantUpwardClimbActivation", "bool")).ToArray();
    }
}
