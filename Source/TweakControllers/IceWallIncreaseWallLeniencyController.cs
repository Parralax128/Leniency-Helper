using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/IceWallIncreaseWallLeniency")]
public class IceWallIncreaseWallLeniencyController : GenericTweakController
{
    public IceWallIncreaseWallLeniencyController(EntityData data, Vector2 offset) : base(data, offset, "IceWallIncreaseWallLeniency")
    {

    }
}