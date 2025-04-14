using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/CustomBufferTime")]
public class CustomBufferTimeController : GenericTweakController
{
    public CustomBufferTimeController(EntityData data, Vector2 offset) : base(data, offset, "CustomBufferTime")
    {

    }
}