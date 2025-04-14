using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/CustomDashbounceTiming")]
public class CustomDashbounceTimingTrigger : GenericTweakTrigger
{
    public CustomDashbounceTimingTrigger(EntityData data, Vector2 offset) : base(data, offset, "CustomDashbounceTiming")
    {

    }
}