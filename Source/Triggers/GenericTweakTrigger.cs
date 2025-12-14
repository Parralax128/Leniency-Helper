using Celeste.Mod.LeniencyHelper.TweakSettings;
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
    bool? savedEnabled;

    Tweak tweak;
    TweakState state;
    
    public GenericTweakTrigger(EntityData data, Vector2 offset, Tweak tweak) : base(data, offset)
    {
        this.tweak = tweak;
        state = TweakData.Tweaks[tweak];

        if (tweak.HasSettings())
            Data = SettingMaster.ParseSettingsFromData(data, tweak);
    }
    protected override void Apply(Player player)
    {
        state.Set(Enabled, SettingSource.Trigger);

        if (Data != null && Enabled)
        {
            for (int c = 0; c < Data.Count; c++)
                TweakData.Tweaks[tweak].Settings.Set(c, SettingSource.Trigger, Data[c]);
        }
    }
    protected override void SaveData()
    {
        savedEnabled = state.Get(SettingSource.Trigger);

        savedData.Clear();

        if (Data == null) return;

        for (int c = 0; c < Data.Count; c++)
        {
            savedData.Add(state.Get(SettingSource.Trigger) == null ?
                state.Settings.Get(c, SettingSource.Controller)
                : state.Settings.Get(c, SettingSource.Trigger));
        }
    }
    protected override void Undo(Player player)
    {
        state.Set(savedEnabled, SettingSource.Trigger);

        if (savedData.Count > 0)
        {
            for (int c = 0; c < Data.Count; c++)
                TweakData.Tweaks[tweak].Settings.Set(c, SettingSource.Trigger, savedData[c]);

            savedData.Clear();
        }
    }
}