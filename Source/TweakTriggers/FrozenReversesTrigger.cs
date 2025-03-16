using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/FrozenReverses")]
public class FrozenReversesTrigger : GenericTweakTrigger
{
    public FrozenReversesTrigger(EntityData data, Vector2 offset, EntityID id) : base(data, offset)
    {
        tweakName = "FrozenReverses";

        fullData = fullData.Append(new TriggerData(0.034f, "reversedFreezeTime", "FreezeTime", "float")).ToArray();
        fullData = fullData.Append(new TriggerData(false, "countReversedInFrames", "CountTimeInFrames", "bool")).ToArray();
    }
}