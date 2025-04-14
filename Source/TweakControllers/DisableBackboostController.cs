using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/DisableBackboost")]
public class DisableBackboostController : GenericTweakController
{
    public DisableBackboostController(EntityData data, Vector2 offset) : base(data, offset, "DisableBackboost")
    {

    }
}