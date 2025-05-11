using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/ManualDreamhyperLeniency")]
public class ManualDreamhyperLeniencyController : GenericTweakController
{
    public ManualDreamhyperLeniencyController(EntityData data, Vector2 offset) : base(data, offset, "ManualDreamhyperLeniency")
    {

    }
}