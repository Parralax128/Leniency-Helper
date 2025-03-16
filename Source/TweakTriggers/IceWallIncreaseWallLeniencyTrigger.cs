using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/IceWallIncreaseWallLeniency")]
public class IceWallIncreaseWallLeniencyTrigger : GenericTweakTrigger
{
    public IceWallIncreaseWallLeniencyTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "IceWallIncreaseWallLeniency";

        fullData = fullData.Append(new TriggerData(3, "iceWJLeniency", "AdditionalLeniency", "int")).ToArray();
    }
}