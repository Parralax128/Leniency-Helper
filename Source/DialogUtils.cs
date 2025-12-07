using Celeste.Mod.LeniencyHelper.Module;
using System;

namespace Celeste.Mod.LeniencyHelper;

public static class DialogUtils
{
    public static string Lookup(string key) => Dialog.Clean(key, Dialog.Languages["english"]);
    public static string Lookup(Tweak tweak) => Lookup($"{LeniencyHelperModule.Name}_Tweaks_{tweak}");
    public static string Lookup(Tweak tweak, string setting) => Lookup($"{LeniencyHelperModule.Name}_Settings_{tweak}_{setting}");
    public static string Lookup(object value) => Lookup($"{LeniencyHelperModule.Name}_Enums_{value.GetType()}_{value}");
    
    public static string Enum<T>(T value)
    {
        Type type = typeof(T);
        return Dialog.Clean($"{LeniencyHelperModule.Name}_Enums_{(type.DeclaringType != null
            ? type.DeclaringType.Name+ '_' + type.Name : type.Name)}_{value}");
    }

    public static string TweakToUrl(Tweak tweak)
    {
        return "https://github.com/Parralax128/Leniency-Helper/wiki/" +
            ToWikiPageName(Lookup(tweak));
    }
    private static string ToWikiPageName(string tweakNameUpper)
    {
        string result = "";

        for (int c = 0; c < tweakNameUpper.Length; c++)
        {
            if (tweakNameUpper[c] == ' ') result += '-';
            else if (tweakNameUpper[c] == '-') result += "%E2%80%90";
            else if (c >= 1 && tweakNameUpper[c - 1] != ' ') result += tweakNameUpper[c].ToString().ToLower();
            else result += tweakNameUpper[c];
        }

        return result;
    }


    public static Time Time(this EntityData data, string key, Time defaultValue)
    {
        //Debug.Warn($"trying to lookup for name \"{key}\" in dialog!");
        string str = data.String(/*DialogLookup(key)*/ key);
        if (string.IsNullOrEmpty(str)) return defaultValue;

        if (str.ToLower().EndsWith('f'))
        {
            if (int.TryParse(str[0..(str.Length - 1)], null, out int frames))
                return new Time(frames, LeniencyHelper.Time.Modes.Frames);

            else throw new ArgumentException($"Invalid time provided: \"{str}\". Could not parse \"{str[0..(str.Length - 1)]}\" as an integer value!");
        }
        else if (float.TryParse(str, null, out float time))
            return new Time(time);

        else throw new ArgumentException($"Invalid time provided: \"{str}\". Could not parse \"{str}\" as a floating-point value!");
    }
}