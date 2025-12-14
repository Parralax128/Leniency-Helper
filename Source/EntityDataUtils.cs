using Celeste.Mod.LeniencyHelper.TweakSettings;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper;

public static class EntityDataUtils
{
    public static List<string> List(this EntityData data, string settingName, params string[] defaultValue)
    {
        if (data.Has(settingName)) return data.String(settingName).Replace(" ", "").
                Split(',', System.StringSplitOptions.RemoveEmptyEntries).ToList();
        
        return defaultValue.ToList();
    }

    public static List<object> ParseTweakSettings(this EntityData data, Tweak tweak)
    {
        if (!tweak.HasSettings()) return null;

        List<object> result = new();

        foreach (AbstractSetting setting in TweakData.Tweaks[tweak].Settings)
        {
            object parsed = setting.ParseFromData(data);
            result.Add(parsed);
        }

        return result;
    }
}