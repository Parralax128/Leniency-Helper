using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/BackwardsRetention")]
public class BackwardsRetentionTrigger : GenericTweakTrigger
{
    public BackwardsRetentionTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "BackwardsRetention";
    }
}