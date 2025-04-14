using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/RetainSpeedCornerboost")]
public class RetainSpeedCornerboostController : GenericTweakController
{
    public RetainSpeedCornerboostController(EntityData data, Vector2 offset) : base(data, offset, "RetainSpeedCornerboost")
    {

    }
}