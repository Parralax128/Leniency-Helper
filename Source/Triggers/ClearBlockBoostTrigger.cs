using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;
using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.Tweaks;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[Tracked]
[CustomEntity("LeniencyHelper/ClearBlockBoostTrigger")]
public class ClearBlockBoostTrigger : GenericTrigger
{
    [OnLoad]
    public static void AddEvent()
    {
        LeniencyHelperModule.BeforePlayerUpdate += ClearBB;
    }
    public static void RemoveEvent()
    {
        LeniencyHelperModule.BeforePlayerUpdate -= ClearBB;
    }

    private static readonly Vector2 FakeZero = new Vector2(0f, 0.0001f);
    private static void ClearBB(Player player)
    {
        if (player.CollideCheck<ClearBlockBoostTrigger>())
        {
            player.currentLiftSpeed = (player.lastLiftSpeed = FakeZero);
            player.liftSpeedTimer = 0f;
        }
    }

    public ClearBlockBoostTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        
    }
    public override void OnEnter(Player player) { }
    public override void OnLeave(Player player)
    {
        if(oneUse) RemoveSelf();
    }
}