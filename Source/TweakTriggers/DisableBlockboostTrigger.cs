using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DisableBackboost")]
public class DisableBackboostTrigger : GenericTweakTrigger
{
    public DisableBackboostTrigger(EntityData data, Vector2 offset) : base(data, offset, "DisableBackboost")
    {

    }
}