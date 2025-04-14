using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/WallCoyoteFrames")]
public class WallCoyoteFramesTrigger : GenericTweakTrigger
{
    public WallCoyoteFramesTrigger(EntityData data, Vector2 offset) : base(data, offset, "WallCoyoteFrames")
    {

    }
}