using System;
using System.Collections.Generic;
using Monocle;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Xml.Linq;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[Tracked(true, Inherited = true)]
public class GenericTweakTrigger : Triggers.GenericTrigger
{
    private Dictionary<string, object> Data = null;
    private Dictionary<string, object> savedValues = new Dictionary<string, object>();
    public string tweakName;

    private bool savedUsedController;

    public GenericTweakTrigger(EntityData data, Vector2 offset, string tweak) : base(data, offset)
    {
        tweakName = tweak;
        Data = SettingMaster.GetSettingsFromData(data, tweak);
    }
    public override void ApplySettings()
    {
        SettingMaster.SetUseController(tweakName, false);
        SettingMaster.SetTriggerTweak(tweakName, enabled);

        if (Data == null || !enabled) return;

        foreach (string checkKey in Data.Keys)
        {
            if (!SettingMaster.TweakSettings[checkKey].triggerValue.Equals(Data[checkKey]))
            {
                foreach (string key in Data.Keys)
                    SettingMaster.SetTriggerSetting(key, Data[key]);   
                
                break;
            }
        }
    }
    public override void GetOldSettings()
    {
        savedValues.Clear();
        savedEnabled = LeniencyHelperModule.Session.Tweaks[tweakName].triggerValue;
        savedUsedController = SettingMaster.Tweaks[tweakName].useController;

        if (Data != null)
        {
            foreach (string key in Data.Keys)
            {
                savedValues.Add(key, SettingMaster.Tweaks[SettingMaster.TweakSettings[key].tweakName].useController ?
                    SettingMaster.TweakSettings[key].controllerValue : SettingMaster.TweakSettings[key].triggerValue);
            }
        }
    }
    public override void UndoSettings()
    {
        SettingMaster.SetTriggerTweak(tweakName, savedEnabled);
        SettingMaster.SetUseController(tweakName, savedUsedController);

        if (savedValues.Count > 0)
        {
            foreach (string name in savedValues.Keys)
            {
                SettingMaster.SetTriggerSetting(name, savedValues[name]);
            }

            savedValues.Clear();
        }
    }
}