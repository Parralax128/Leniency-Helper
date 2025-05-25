using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/ConsistentWallboosters")]
public class ConsistentWallboostersController : GenericTweakController
{ 
    public ConsistentWallboostersController(EntityData data, Vector2 offset) : base(data, offset, "ConsistentWallboosters")
    {

    }
}