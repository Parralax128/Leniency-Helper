using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/ConsistentDashOnDBlockExit")]
public class ConsistentDashOnDBlockExitController : GenericTweakController
{
    public ConsistentDashOnDBlockExitController(EntityData data, Vector2 offset) : base(data, offset, "ConsistentDashOnDBlockExit")
    {

    }
}