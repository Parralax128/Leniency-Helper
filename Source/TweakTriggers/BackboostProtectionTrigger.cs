using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/BackboostProtection")]
public class BackboostProtectionTrigger : GenericTweakTrigger
{
    public BackboostProtectionTrigger(EntityData data, Vector2 offset) : base(data, offset, "BackboostProtection")
    {

    }
}