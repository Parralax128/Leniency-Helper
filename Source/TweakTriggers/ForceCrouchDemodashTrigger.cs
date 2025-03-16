using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/ForceCrouchDemodash")]
public class ForceCrouchDemodashTrigger : GenericTweakTrigger
{
    public ForceCrouchDemodashTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "ForceCrouchDemodash";
    }
}