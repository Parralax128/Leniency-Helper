using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DynamicCornerCorrection")]
public class DynamicCornerCorrectionTrigger : GenericTweakTrigger
{
    public DynamicCornerCorrectionTrigger(EntityData data, Vector2 offset) : base(data, offset, "DynamicCornerCorrection")
    {

    }
}