using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/InstantClimbHop")]
public class InstantClimbHopController : GenericTweakController
{
    public InstantClimbHopController(EntityData data, Vector2 offset) : base(data, offset, "InstantClimbHop")
    {

    }
}