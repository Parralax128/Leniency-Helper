using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/CornerWaveLeniency")]
public class CornerWaveLeniencyController : GenericTweakController
{
    public CornerWaveLeniencyController(EntityData data, Vector2 offset) : base(data, offset, "CornerWaveLeniency")
    {

    }
}