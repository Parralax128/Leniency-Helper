using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DisableForcemovedTech")]
public class DisableForcemovedTechTrigger : GenericTweakTrigger
{
    public DisableForcemovedTechTrigger(EntityData data, Vector2 offset) : base(data, offset, "DisableForcemovedTech")
    {

    }
}