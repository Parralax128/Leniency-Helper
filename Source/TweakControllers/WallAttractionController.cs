using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/WallAttraction")]
public class WallAttractionController : GenericTweakController
{
    public WallAttractionController(EntityData data, Vector2 offset) : base(data, offset, "WallAttraction")
    {

    }
}