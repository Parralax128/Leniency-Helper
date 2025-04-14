using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/ForceCrouchDemodash")]
public class ForceCrouchDemodashController : GenericTweakController
{
    public ForceCrouchDemodashController(EntityData data, Vector2 offset) : base(data, offset, "ForceCrouchDemodash")
    {

    }
}