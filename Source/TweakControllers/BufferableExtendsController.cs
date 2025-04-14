using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/BufferableExtends")]
public class BufferableExtendsController : GenericTweakController
{
    public BufferableExtendsController(EntityData data, Vector2 offset) : base(data, offset, "BufferableExtends")
    {

    }
}