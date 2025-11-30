using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper.Controllers;

[Tracked(true, Inherited = true)]
public class GenericTweakController : GenericController
{
    public Tweak tweak;
    private Dictionary<string, object> Data = null;
    private Dictionary<string, object> savedData = new Dictionary<string, object>();
    private bool savedEnabled;

    public GenericTweakController(EntityData data, Vector2 offset, Tweak tweak) : base(data, offset, true)
    {
        this.tweak = tweak;
        if (tweak.HasSettings())
            Data = SettingMaster.ParseSettingsFromData(data, tweak);
    }
    public override void GetOldSettings()
    {
        savedEnabled = TweakData.Tweaks[tweak].Get(TweakSettings.SettingSource.Controller) == true;

        if (Data == null) return;

        foreach (string key in Data.Keys)
        {
            savedData.Add(key, TweakData.Tweaks[tweak].Settings.Get(key, TweakSettings.SettingSource.Controller));
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
            TweakData.Tweaks[tweak].Settings.Set(key, TweakSettings.SettingSource.Controller, Data[key]);
    }
    public void ApplyTweak()
    {
        SettingMaster.SetControllerTweak(tweak, true);
    }

    public void UndoSettings()
    {
        if (savedData.Count() > 0)
        {
            foreach (string key in savedData.Keys)
            {
                try { TweakData.Tweaks[tweak].Settings.Set(key, TweakSettings.SettingSource.Controller, savedData[key]); }
                catch (Exception e)
                {
                    Debug.Warn($"Could not set {tweak}.{key} to {savedData[key] ?? "null"}!");
                    Debug.Warn(e);
                }
            }
              
        }
    }
    public void UndoTweak()
    {
        SettingMaster.SetControllerTweak(tweak, savedEnabled);
    }
}