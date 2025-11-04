using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/RespectInputOrder")]
public class RespectInputOrderController : GenericTweakController
{
    public RespectInputOrderController(EntityData data, Vector2 offset) : base(data, offset, "RespectInputOrder")
    {

    }
}