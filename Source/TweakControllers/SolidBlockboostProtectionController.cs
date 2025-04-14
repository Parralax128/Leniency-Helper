using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/SolidBlockboostProtection")]
public class SolidBlockboostProtectionController : GenericTweakController
{
    public SolidBlockboostProtectionController(EntityData data, Vector2 offset) : base(data, offset, "SolidBlockboostProtection")
    {

    }
}