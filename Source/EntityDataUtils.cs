using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper;

public static class EntityDataUtils
{
    public static List<string> List(this EntityData data, string settingName, params string[] defaultValue)
    {
        if (data.Has(settingName)) return data.String(settingName).Replace(" ", "").
                Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        
        return defaultValue.ToList();
    }

    public static List<object> ExtractSettings(this EntityData data, Tweak tweak) => 
        TweakData.Tweaks[tweak].Settings?.Select(s => s.ParseFromData(data, tweak)).ToList();

    public static Time Time(this EntityData data, string key, Time defaultValue)
    {
        string str = data.String(key);
        if (string.IsNullOrEmpty(str)) return defaultValue;

        if (str.StartsWith('-'))
            throw new ArgumentException($"Invalid time setting provided: \"{str}\". Time cannot be negative!");

        if (str.ToLower().EndsWith('f'))
        {
            if (int.TryParse(str[0..(str.Length - 1)], null, out int frames))
                return new Time(frames, LeniencyHelper.Time.Modes.Frames);

            else throw new ArgumentException($"Invalid time setting provided: \"{str}\"." +
                $" Could not parse \"{str[0..(str.Length - 1)]}\" as an integer value (frame count)!");
        }
        else if (float.TryParse(str, null, out float time) || (str.ToLower().EndsWith('s')
            && float.TryParse(str[0..(str.Length - 1)], null, out float seconds)))
            return new Time(time);

        else throw new ArgumentException($"Invalid time setting provided: \"{str}\"." +
            $" Could not parse \"{str}\" as a floating-point value" +
            $" or \"{str[0..(str.Length - 1)]}\" as time in seconds!");
    }
}