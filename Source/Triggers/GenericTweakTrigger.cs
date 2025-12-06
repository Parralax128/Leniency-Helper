using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[Tracked(true, Inherited = true)]
public class GenericTweakTrigger : GenericTrigger
{
    private Dictionary<string, object> Data = null;
    private Dictionary<string, object> savedData = new();
    private Tweak tweak;
    private bool? savedEnabled;

    public GenericTweakTrigger(EntityData data, Vector2 offset, Tweak tweak) : base(data, offset)
    {
        this.tweak = tweak;
        if (tweak.HasSettings())
            Data = SettingMaster.ParseSettingsFromData(data, tweak);
    }
    public override void ApplySettings()
    {
        SettingMaster.SetTriggerTweak(tweak, enabled);

        if (Data != null && enabled)
        {
            foreach (string key in Data.Keys)
                tweak.SetSetting(key, Data[key], TweakSettings.SettingSource.Trigger);
        }
    }
    public override void GetOldSettings()
    {
        TweakSettings.TweakState tweakState = TweakData.Tweaks[tweak];
        savedEnabled = tweakState.Get(TweakSettings.SettingSource.Trigger);

        savedData.Clear();

        if (Data == null) return;
        
        foreach (string key in Data.Keys)
        {
            savedData.Add(key, tweakState.Get(TweakSettings.SettingSource.Trigger) == null ?
                tweakState.Settings.Get(key, TweakSettings.SettingSource.Controller)
                : tweakState.Settings.Get(key, TweakSettings.SettingSource.Trigger));
        }
    }
    public override void UndoSettings()
    {
        SettingMaster.SetTriggerTweak(tweak, savedEnabled);

        if (savedData.Count > 0)
        {
            foreach (string key in savedData.Keys)
            {
                TweakData.Tweaks[tweak].Settings.Set(key, TweakSettings.SettingSource.Trigger, savedData[key]);
            }

            savedData.Clear();
        }
    }
}