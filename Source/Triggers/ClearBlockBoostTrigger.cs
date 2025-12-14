using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.Triggers;


[CustomEntity("LeniencyHelper/ClearBlockBoostTrigger")]
class ClearBlockBoostTrigger : GenericTrigger
{
    static readonly Vector2 FakeZero = new Vector2(0f, 0.0001f);

    public ClearBlockBoostTrigger(EntityData data, Vector2 offset) : base(data, offset, applyOnStay: true) { }

    protected override void Apply(Player player)
    {
        player.currentLiftSpeed = player.lastLiftSpeed = FakeZero;
        player.liftSpeedTimer = 0f;
    }
}