using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper.Controllers;

[Tracked(true, Inherited = true)]
class GenericTweakController : GenericController
{
    public Tweak tweak;

    List<object> Data = null;
    List<object> savedData = new();
    bool savedEnabled;

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
        
        for(int c=0; c<Data.Count; c++) {
            savedData.Add(TweakData.Tweaks[tweak].Settings.Get(c, TweakSettings.SettingSource.Controller));
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

        for (int c = 0; c < Data.Count; c++)
            TweakData.Tweaks[tweak].Settings.Set(c, TweakSettings.SettingSource.Controller, Data[c]);
    }
    public void ApplyTweak()
    {
        SettingMaster.SetControllerTweak(tweak, true);
    }

    public void UndoSettings()
    {
        if (savedData.Count() > 0)
            for (int c = 0; c < Data.Count; c++)
                TweakData.Tweaks[tweak].Settings.Set(c, TweakSettings.SettingSource.Controller, savedData[c]);
    }
    public void UndoTweak()
    {
        SettingMaster.SetControllerTweak(tweak, savedEnabled);
    }
}