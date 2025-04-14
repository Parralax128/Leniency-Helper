using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/SuperOverWalljump")]
public class SuperOverWalljumpController : GenericTweakController
{
    public SuperOverWalljumpController(EntityData data, Vector2 offset) : base(data, offset, "SuperOverWalljump")
    {

    }
}