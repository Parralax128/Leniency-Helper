using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/RefillDashInCoyote")]
public class RefillDashInCoyoteController : GenericTweakController
{
    public RefillDashInCoyoteController(EntityData data, Vector2 offset) : base(data, offset, "RefillDashInCoyote")
    {

    }
}