using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/NoFailedTech")]
public class NoFailedTechController : GenericTweakController
{
    public NoFailedTechController(EntityData data, Vector2 offset) : base(data, offset, "NoFailedTech")
    {

    }
}