using System.Collections.Generic;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[Tracked(true, Inherited = true)]
public class GenericTweakTrigger : GenericTrigger
{
    private Dictionary<string, object> Data = null;
    private Dictionary<string, object> savedValues = new Dictionary<string, object>();
    public Tweak tweak;

    private bool savedUsedController;
    private bool savedEnabled;

    public GenericTweakTrigger(EntityData data, Vector2 offset, Tweak tweak) : base(data, offset)
    {
        this.tweak = tweak;
        if (SettingMaster.AssociatedSettings[tweak] != null)
            Data = SettingMaster.GetSettingsFromData(data, tweak);
    }
    public override void ApplySettings()
    {
        SettingMaster.SetUseController(tweak, false);
        SettingMaster.SetTriggerTweak(tweak, enabled);

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
        savedEnabled = LeniencyHelperModule.Session.TriggerTweaks[tweak];

        savedValues.Clear();
        savedUsedController = LeniencyHelperModule.Session.UseController[tweak];

        if (Data == null) return;
        
        foreach (string key in Data.Keys)
        {
            savedValues.Add(key, LeniencyHelperModule.Session.UseController[tweak] ?
                SettingMaster.GetControllerSetting(key) : SettingMaster.GetTriggerSetting(key));
        }
    }
    public override void UndoSettings()
    {
        SettingMaster.SetTriggerTweak(tweak, savedEnabled);
        SettingMaster.SetUseController(tweak, savedUsedController);

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