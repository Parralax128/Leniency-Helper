using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/BackboostProtection")]
public class BackboostProtectionController : GenericTweakController
{
    public BackboostProtectionController(EntityData data, Vector2 offset) : base(data, offset, "BackboostProtection")
    {
        
    }
}