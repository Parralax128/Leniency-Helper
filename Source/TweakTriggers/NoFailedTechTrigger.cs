using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/NoFailedTech")]
public class NoFailedTechTrigger : GenericTweakTrigger
{
    public NoFailedTechTrigger(EntityData data, Vector2 offset) : base(data, offset, "NoFailedTech")
    {

    }
}