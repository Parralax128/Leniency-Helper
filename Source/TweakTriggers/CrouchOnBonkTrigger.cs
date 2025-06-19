using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/CrouchOnBonk")]
public class CrouchOnBonkTrigger : GenericTweakTrigger
{
    public CrouchOnBonkTrigger(EntityData data, Vector2 offset) : base(data, offset, "CrouchOnBonk")
    {

    }
}