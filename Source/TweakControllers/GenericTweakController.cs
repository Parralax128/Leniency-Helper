using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[Tracked(true, Inherited = true)]
public class GenericTweakController : Controllers.GenericController
{
    public string tweakName;
    private Dictionary<string, object> Data = null;
    private Dictionary<string, object> savedData = new Dictionary<string, object>();
    private bool savedEnabled;

    public GenericTweakController(EntityData data, Vector2 offset, string tweak) : base(data, offset)
    {
        tweakName = tweak;
        if (SettingMaster.AssociatedTweaks[tweak] != null)
            Data = SettingMaster.GetSettingsFromData(data, tweak);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);

        SettingMaster.SetUseController(tweakName, true);

        foreach (GenericTweakController gtc in SceneAs<Level>().Tracker.GetEntities<GenericTweakController>())
        {
            if (!gtc.Equals(this) && gtc.GetType() == this.GetType())
            {
                gtc.RemoveSelf();
            }
        }
    }
    public override void GetOldSettings()
    {
        savedEnabled = LeniencyHelperModule.Session.ControllerTweaks[tweakName];

        if (Data == null) return;

        foreach (string key in Data.Keys)
        {
            savedData.Add(key, SettingMaster.GetControllerSetting(key));
        }
    }
    public override void ApplySettings()
    {
        if (Data == null) return;
        
        foreach (string key in Data.Keys)
            SettingMaster.SetControllerSetting(key, Data[key]);
    }
    public override void ApplyTweak()
    {
        SettingMaster.SetControllerTweak(tweakName, true);
    }

    public override void UndoSettings()
    {
        if (savedData.Count() > 0)
        {
            foreach (string currentSetting in savedData.Keys)
                SettingMaster.SetControllerSetting(currentSetting, savedData[currentSetting]);
        }        
    }
    public override void UndoTweak()
    {
        SettingMaster.SetControllerTweak(tweakName, savedEnabled);
    }
}