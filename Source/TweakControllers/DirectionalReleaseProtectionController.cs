using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/DirectionalReleaseProtection")]
public class DirectionalReleaseProtectionController : GenericTweakController
{
    public DirectionalReleaseProtectionController(EntityData data, Vector2 offset) : base(data, offset, "DirectionalReleaseProtection")
    {

    }
}