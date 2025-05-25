using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/ConsistentWallboosters")]
public class ConsistentWallboostersTrigger : GenericTweakTrigger
{
    public ConsistentWallboostersTrigger(EntityData data, Vector2 offset) : base(data, offset, "ConsistentWallboosters")
    {

    }
}