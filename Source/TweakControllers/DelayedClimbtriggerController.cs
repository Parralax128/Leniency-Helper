using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/DelayedClimbtrigger")]
public class DelayedClimbtriggerController : GenericTweakController
{
    public DelayedClimbtriggerController(EntityData data, Vector2 offset) : base(data, offset, "DelayedClimbtrigger")
    {

    }
}