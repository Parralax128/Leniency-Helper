using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/ReverseLeniency")]
public class ReverseLeniencyController : GenericTweakController
{
    public ReverseLeniencyController(EntityData data, Vector2 offset) : base(data, offset, "ReverseLeniency")
    {

    }
}