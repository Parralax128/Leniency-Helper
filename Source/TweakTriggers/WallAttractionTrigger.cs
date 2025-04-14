using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/WallAttraction")]
public class WallAttractionTrigger : GenericTweakTrigger
{
    public WallAttractionTrigger(EntityData data, Vector2 offset) : base(data, offset, "WallAttraction")
    {

    }
}