using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/RemoveDBlockCCorection")]
public class RemoveDBlockCCorectionTrigger : GenericTweakTrigger
{
    public RemoveDBlockCCorectionTrigger(EntityData data, Vector2 offset) : base(data, offset, "RemoveDBlockCCorection")
    {

    }
}