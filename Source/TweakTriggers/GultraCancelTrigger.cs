using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/GultraCancel")]
public class GultraCancelTrigger : GenericTweakTrigger
{
    public GultraCancelTrigger(EntityData data, Vector2 offset) : base(data, offset, "GultraCancel")
    {

    }
}