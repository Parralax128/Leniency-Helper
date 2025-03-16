using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DirectionalReleaseProtection")]
public class DirectionalReleaseProtectionTrigger : GenericTweakTrigger
{
    private Dirs dataJumpDir, dataDashDir;
    public DirectionalReleaseProtectionTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "DirectionalReleaseProtection";

        fullData = fullData.Append(new TriggerData(0.1f, "DirectionalBufferTime", "BufferTime", "float")).ToArray();
        fullData = fullData.Append(new TriggerData(false, "CountInFrames", "CountProtectionTimeInFrames", "bool")).ToArray();

        dataJumpDir = data.Enum("ProtectedDashDirections", Dirs.Down);
        dataDashDir = data.Enum("ProtectedJumpDirections", Dirs.All);
    }
    public override void ApplySettings()
    {
        base.ApplySettings();

        var s = LeniencyHelperModule.Session;
        s.mapJumpDir = dataJumpDir;
        s.mapDashDir = dataDashDir;
    }
}