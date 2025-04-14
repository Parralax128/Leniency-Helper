using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/WallCoyoteFrames")]
public class WallCoyoteFramesController : GenericTweakController
{
    public WallCoyoteFramesController(EntityData data, Vector2 offset) : base(data, offset, "WallCoyoteFrames")
    {

    }
}