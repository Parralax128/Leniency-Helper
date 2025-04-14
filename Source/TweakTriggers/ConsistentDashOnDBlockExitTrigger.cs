using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/ConsistentDashOnDBlockExit")]
public class ConsistentDashOnDBlockExitTrigger : GenericTweakTrigger
{
    public ConsistentDashOnDBlockExitTrigger(EntityData data, Vector2 offset) : base(data, offset, "ConsistentDashOnDBlockExit")
    {

    }
}