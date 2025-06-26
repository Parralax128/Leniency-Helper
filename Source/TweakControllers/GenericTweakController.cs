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

    public GenericTweakController(EntityData data, Vector2 offset, string tweak) : base(data, offset, true)
    {
        tweakName = tweak;
        if (SettingMaster.AssociatedSettings[tweak] != null)
            Data = SettingMaster.GetSettingsFromData(data, tweak);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);

        SettingMaster.SetUseController(tweakName, true);
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

    public override void Apply(bool fromFlag)
    {
        ApplyTweak();
        if(!fromFlag) ApplySettings();
    }
    public override void Undo(bool fromFlag)
    {
        UndoTweak();
        if (!fromFlag) UndoSettings();
    }

    public void ApplySettings()
    {
        if (Data == null) return;
        
        foreach (string key in Data.Keys)
            SettingMaster.SetControllerSetting(key, Data[key]);
    }
    public void ApplyTweak()
    {
        SettingMaster.SetControllerTweak(tweakName, true);
    }

    public void UndoSettings()
    {
        if (savedData.Count() > 0)
        {
            foreach (string currentSetting in savedData.Keys)
                SettingMaster.SetControllerSetting(currentSetting, savedData[currentSetting]);
        }        
    }
    public void UndoTweak()
    {
        SettingMaster.SetControllerTweak(tweakName, savedEnabled);
    }
}