using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/InstantAcceleratedJumps")]
public class InstantAcceleratedJumpsController : GenericTweakController
{
    public InstantAcceleratedJumpsController(EntityData data, Vector2 offset) : base(data, offset, "InstantAcceleratedJumps")
    {

    }
}