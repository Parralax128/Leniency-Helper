using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/BufferableExtends")]
public class BufferableExtendsTrigger : GenericTweakTrigger
{
    public BufferableExtendsTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "BufferableExtends";

        fullData = fullData.Append(new TriggerData(false, "forceWaitForRefill", "ForceWaitForRefill", "bool")).ToArray();
    }
}
