using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/CustomDashbounceTiming")]
public class CustomDashbounceTimingController : GenericTweakController
{
    public CustomDashbounceTimingController(EntityData data, Vector2 offset) : base(data, offset, "CustomDashbounceTiming")
    {

    }
}