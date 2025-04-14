using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/RemoveDBlockCCorection")]
public class RemoveDBlockCCorectionController : GenericTweakController
{
    public RemoveDBlockCCorectionController(EntityData data, Vector2 offset) : base(data, offset, "RemoveDBlockCCorection")
    {

    }
}