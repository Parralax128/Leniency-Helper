using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/GultraCancel")]
public class GultraCancelController : GenericTweakController
{
    public GultraCancelController(EntityData data, Vector2 offset) : base(data, offset, "GultraCancel")
    {

    }
}