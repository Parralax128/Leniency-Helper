using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/BufferableClimbtrigger")]
public class BufferableClimbtriggerController : GenericTweakController
{
    public BufferableClimbtriggerController(EntityData data, Vector2 offset) : base(data, offset, "BufferableClimbtrigger")
    {
        
    }
}