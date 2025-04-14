using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/DynamicWallLeniency")]
public class DynamicWallLeniencyController : GenericTweakController
{
    public DynamicWallLeniencyController(EntityData data, Vector2 offset) : base(data, offset, "DynamicWallLeniency")
    {

    }
}