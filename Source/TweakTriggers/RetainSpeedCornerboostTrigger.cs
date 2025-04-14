using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/RetainSpeedCornerboost")]
public class RetainSpeedCornerboostTrigger : GenericTweakTrigger
{
    public RetainSpeedCornerboostTrigger(EntityData data, Vector2 offset) : base(data, offset, "RetainSpeedCornerboost")
    {

    }
}