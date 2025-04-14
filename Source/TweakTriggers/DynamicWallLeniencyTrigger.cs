using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DynamicWallLeniency")]
public class DynamicWallLeniencyTrigger : GenericTweakTrigger
{
    public DynamicWallLeniencyTrigger(EntityData data, Vector2 offset) : base(data, offset, "DynamicWallLeniency")
    {

    }
}