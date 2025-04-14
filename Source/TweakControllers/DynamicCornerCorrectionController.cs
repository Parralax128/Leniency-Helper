using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/DynamicCornerCorrection")]
public class DynamicCornerCorrectionController : GenericTweakController
{
    public DynamicCornerCorrectionController(EntityData data, Vector2 offset) : base(data, offset, "DynamicCornerCorrection")
    {

    }
}