using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/CustomSnapDownDistance")]
public class CustomSnapDownDistanceTrigger : GenericTweakTrigger
{
    public CustomSnapDownDistanceTrigger(EntityData data, Vector2 offset) : base(data, offset, "CustomSnapDownDistance")
    {

    }
}