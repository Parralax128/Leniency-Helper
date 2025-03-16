using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DynamicCornerCorrection")]
public class DynamicCornerCorrectionTrigger : GenericTweakTrigger
{
    public DynamicCornerCorrectionTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "DynamicCornerCorrection";

        fullData = fullData.Append(new TriggerData(0.1f, "FloorCorrectionTiming", "float")).ToArray();
        fullData = fullData.Append(new TriggerData(0.05f, "WallCorrectionTiming", "float")).ToArray();
        fullData = fullData.Append(new TriggerData(false, "ccorectionTimingInFrames", "CountInFrames", "bool")).ToArray();
    }
}