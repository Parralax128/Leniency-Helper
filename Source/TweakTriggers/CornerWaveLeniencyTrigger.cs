using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/CornerWaveLeniency")]
public class CornerWaveLeniencyTrigger : GenericTweakTrigger
{
    public CornerWaveLeniencyTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "CornerWaveLeniency";

        fullData = fullData.Append(new TriggerData(false, "allowSpikedFloor", "AllowSpikedFloor", "bool")).ToArray();
    }
}