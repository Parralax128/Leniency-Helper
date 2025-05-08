using System.Linq;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/DirectionalReleaseProtection")]
public class DirectionalReleaseProtectionTrigger : GenericTweakTrigger
{
    private Dirs dataJumpDir, dataDashDir;
    public DirectionalReleaseProtectionTrigger(EntityData data, Vector2 offset) : base(data, offset, "DirectionalReleaseProtection")
    {
        dataJumpDir = data.Enum("ProtectedDashDirections", Dirs.Down);
        dataDashDir = data.Enum("ProtectedJumpDirections", Dirs.All);
    }
    public override void ApplySettings()
    {
        base.ApplySettings();
        SettingMaster.SetTriggerSetting("jumpDir", dataJumpDir);
        SettingMaster.SetTriggerSetting("dashDir", dataDashDir);
    }
}