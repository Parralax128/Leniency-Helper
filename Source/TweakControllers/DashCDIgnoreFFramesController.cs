using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/DashCDIgnoreFFrames")]
public class DashCDIgnoreFFramesController : GenericTweakController
{
    public DashCDIgnoreFFramesController(EntityData data, Vector2 offset) : base(data, offset, "DashCDIgnoreFFrames")
    {

    }
}