using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.TweakSettings;


namespace Celeste.Mod.LeniencyHelper;
public static class SettingMaster
{
    
    

    public static List<object> ParseSettingsFromData(EntityData data, Tweak tweak)
    {
        if (!tweak.HasSettings()) return null;

        List<object> result = new();

        foreach (AbstractSetting setting in TweakData.Tweaks[tweak].Settings)
        {
            object parsed = setting.ParseFromData(data);
            if (parsed is List<object> innerSettings)
                result.AddRange(innerSettings);
            else result.Add(parsed);
        }

        return result;
    }
}