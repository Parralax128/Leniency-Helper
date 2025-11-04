using System.Collections.Generic;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[Tracked(true, Inherited = true)]
public class GenericTweakTrigger : Triggers.GenericTrigger
{
    private Dictionary<string, object> Data = null;
    private Dictionary<string, object> savedValues = new Dictionary<string, object>();
    public string tweakName;

    private bool savedUsedController;
    private bool savedEnabled;

    public GenericTweakTrigger(EntityData data, Vector2 offset, string tweak) : base(data, offset)
    {
        tweakName = tweak;
        if (SettingMaster.AssociatedSettings[tweak] != null)
            Data = SettingMaster.GetSettingsFromData(data, tweak);
    }
    public override void ApplySettings()
    {
        SettingMaster.SetUseController(tweakName, false);
        SettingMaster.SetTriggerTweak(tweakName, enabled);

        if (Data == null || !enabled) return;

        foreach (string checkKey in Data.Keys)
        {
            if (!SettingMaster.GetTriggerSetting(checkKey).Equals(Data[checkKey]))
            {
                foreach (string key in Data.Keys)
                    SettingMaster.SetTriggerSetting(key, Data[key]);   
                
                break;
            }
        }
    }
    public override void GetOldSettings()
    {
        savedEnabled = LeniencyHelperModule.Session.TriggerTweaks[tweakName];

        savedValues.Clear();
        savedUsedController = LeniencyHelperModule.Session.UseController[tweakName];

        if (Data == null) return;
        
        foreach (string key in Data.Keys)
        {
            savedValues.Add(key, LeniencyHelperModule.Session.UseController[tweakName] ?
                SettingMaster.GetControllerSetting(key) : SettingMaster.GetTriggerSetting(key));
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