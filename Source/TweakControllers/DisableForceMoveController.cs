using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/DisableForceMove")]
public class DisableForceMoveController : GenericTweakController
{
    public DisableForceMoveController(EntityData data, Vector2 offset) : base(data, offset, "DisableForceMove")
    {

    }
}