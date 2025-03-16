using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DashCDIgnoreFFrames")]
public class DashCDIgnoreFFramesTrigger : GenericTweakTrigger
{
    public DashCDIgnoreFFramesTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "DashCDIgnoreFFrames";
    }
}