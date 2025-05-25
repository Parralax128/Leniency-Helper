using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/SuperOverWalljump")]
public class SuperOverWalljumpTrigger : GenericTweakTrigger
{
    public SuperOverWalljumpTrigger(EntityData data, Vector2 offset) : base(data, offset, "SuperOverWalljump")
    {

    }
}