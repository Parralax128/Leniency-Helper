using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[Tracked(true, Inherited = true)]
class GenericTweakTrigger : GenericTrigger
{
    List<object> Data = null;
    List<object> savedData = new();
    Tweak tweak;
    bool? savedEnabled;

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
            for (int c = 0; c < Data.Count; c++)
                TweakData.Tweaks[tweak].Settings.Set(c, TweakSettings.SettingSource.Trigger, Data[c]);
        }
    }
    public override void GetOldSettings()
    {
        TweakSettings.TweakState tweakState = TweakData.Tweaks[tweak];
        savedEnabled = tweakState.Get(TweakSettings.SettingSource.Trigger);

        savedData.Clear();

        if (Data == null) return;

        for (int c = 0; c < Data.Count; c++)
        {
            savedData.Add(tweakState.Get(TweakSettings.SettingSource.Trigger) == null ?
                tweakState.Settings.Get(c, TweakSettings.SettingSource.Controller)
                : tweakState.Settings.Get(c, TweakSettings.SettingSource.Trigger));
        }
    }
    public override void UndoSettings()
    {
        SettingMaster.SetTriggerTweak(tweak, savedEnabled);

        if (savedData.Count > 0)
        {
            for (int c = 0; c < Data.Count; c++)
                TweakData.Tweaks[tweak].Settings.Set(c, TweakSettings.SettingSource.Trigger, savedData[c]);

            savedData.Clear();
        }
    }
}