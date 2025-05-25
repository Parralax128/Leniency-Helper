using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/CustomSnapDownDistance")]
public class CustomSnapDownDistanceController : GenericTweakController
{
    public CustomSnapDownDistanceController(EntityData data, Vector2 offset) : base(data, offset, "CustomSnapDownDistance")
    {
        
    }
}