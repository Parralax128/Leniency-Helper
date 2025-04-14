using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/AutoSlowfall")]
public class AutoSlowfallTrigger : GenericTweakTrigger
{
    public AutoSlowfallTrigger(EntityData data, Vector2 offset) : base(data, offset, "AutoSlowfall")
    {

    }
}