using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/ExtendBufferOnFreezeAndPickup")]
public class ExtendBufferOnFreezeAndPickupTrigger : GenericTweakTrigger
{
    public ExtendBufferOnFreezeAndPickupTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "ExtendBufferOnFreezeAndPickup";

        fullData = fullData.Append(new TriggerData(true, "ExtendBufferOnFreeze", "bool")).ToArray();
        fullData = fullData.Append(new TriggerData(true, "ExtendBufferOnPickup", "bool")).ToArray();
    }
}