using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/CrouchOnBonk")]
public class CrouchOnBonkController : GenericTweakController
{
    public CrouchOnBonkController(EntityData data, Vector2 offset) : base(data, offset, "CrouchOnBonk")
    {

    }
}