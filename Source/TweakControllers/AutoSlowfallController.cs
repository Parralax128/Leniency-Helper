using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/AutoSlowfall")]
public class AutoSlowfallController : GenericTweakController
{
    public AutoSlowfallController(EntityData data, Vector2 offset) : base(data, offset, "AutoSlowfall")
    {

    }
}