using Celeste.Mod.LeniencyHelper.Module;
using System;

namespace Celeste.Mod.LeniencyHelper;

public static class DialogUtils
{
    static string LeniencyHelper = LeniencyHelperModule.Name;
    public enum Precision
    {
        Localized,
        EnglishOnly,
        ImmutableKey
    }
    public static string Tweak(Tweak tweak, Precision level)
    {
        return level switch
        {
            Precision.Localized => Dialog.Clean($"{LeniencyHelper}_Tweaks_{tweak}"),
            Precision.EnglishOnly => Dialog.Clean($"{LeniencyHelper}_Tweaks_{tweak}", Dialog.Languages["english"]),
            _ => null
        };
    }
    public static string Setting(string setting, Tweak tweak, Precision level)
    {
        return level switch
        {
            Precision.Localized => Dialog.Clean($"{LeniencyHelper}_Settings_{tweak}_{setting}"),
            Precision.EnglishOnly => Dialog.Clean($"{LeniencyHelper}_Settings_{tweak}_{setting}", Dialog.Languages["english"]),
            Precision.ImmutableKey => Dialog.Clean($"{LeniencyHelper}_Lookup_{tweak}_{setting}", Dialog.Languages["english"]),
            _ => null
        };
    }
    public static string Enum<T>(T value, Precision level)
    {
        return level switch
        {
            Precision.Localized => Dialog.Clean($"{LeniencyHelper}_{GetEnumID(value)}"),
            Precision.EnglishOnly => Dialog.Clean($"{LeniencyHelper}_{GetEnumID(value)}", Dialog.Languages["english"]),
            _ => null
        };
    }
    static string GetEnumID<T>(T value)
    {
        Type type = typeof(T);
        return $"Enums_{(type.DeclaringType != null ? type.DeclaringType.Name + '_' + type.Name : type.Name)}_{value}";
    }


    public static string TweakToUrl(Tweak tweak)
    {
        return "https://github.com/Parralax128/Leniency-Helper/wiki/" +
            ToWikiPageName(Tweak(tweak, Precision.EnglishOnly));
    }
    static string ToWikiPageName(string tweakNameUpper)
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
}