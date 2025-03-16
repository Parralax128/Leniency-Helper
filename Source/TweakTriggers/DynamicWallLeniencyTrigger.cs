using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DynamicWallLeniency")]
public class DynamicWallLeniencyTrigger : GenericTweakTrigger
{
    public DynamicWallLeniencyTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "DynamicWallLeniency";

        fullData = fullData.Append(new TriggerData(0.1f, "wallLeniencyTiming", "TimingWindow", "float")).ToArray();
        fullData = fullData.Append(new TriggerData(false, "countWallTimingInFrames", "CountInFrames", "bool")).ToArray();
    }
}