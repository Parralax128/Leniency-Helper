using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/RespectInputOrder")]
public class RespectInputOrderTrigger : GenericTweakTrigger
{
    public RespectInputOrderTrigger(EntityData data, Vector2 offset) : base(data, offset, "RespectInputOrder")
    {

    }
}