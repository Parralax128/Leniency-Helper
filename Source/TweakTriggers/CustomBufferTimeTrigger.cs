using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/CustomBufferTime")]
public class CustomBufferTimeTrigger : GenericTweakTrigger
{
    public CustomBufferTimeTrigger(EntityData data, Vector2 offset) : base(data, offset, "CustomBufferTime")
    {

    }
}