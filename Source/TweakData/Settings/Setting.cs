using System.Collections.Generic;
using System;

namespace Celeste.Mod.LeniencyHelper.TweakData.Settings;
public class Setting<T>
{
    public string Name;

    public Dictionary<SettingSource, T> Values = new();

    private Bounds<T> ValueBounds = null;

    private List<T> ValidLeniencyValues = null;
    private Bounds<T> ValidLeniencyBounds = null;

    public Setting(string name, T defaultValue)
    {
        Name = name;

        // fulfill values
        foreach(var source in Enum.GetValues<SettingSource>())
        {
            Values[source] = defaultValue;
        }
    }
    public Setting(string name, T defaultValue, T min, T max) : this(name, defaultValue)
    {
        ValueBounds = new Bounds<T>(min, max);
    }

    public virtual bool? Set(SettingSource source, T value)
    {
        bool? result = true;
        if (ValueBounds != null && !ValueBounds.Check(value)) result = null;
        else if (source == SettingSource.Player && (ValidLeniencyBounds != null || ValidLeniencyValues != null))
        {
            result = ValidLeniencyBounds?.Check(value) ?? ValidLeniencyValues.Contains(value);
        }
        
        if(result == true)
        {
            Values[source] = value;
        }
        
        return result;
    }
    public virtual T Get(SettingSource source, params object[] extra)
    {
        return Values[source];
    }

    public virtual bool? Reset(SettingSource source)
        => Set(source, Values[SettingSource.Default]);


    public void SetValidLeniency(T min, T max)
    {        
        ValidLeniencyBounds = new Bounds<T>(min, max);
        ValidLeniencyValues = null;
    }
    public void SetValidLeniency(List<T> validValues)
    {
        ValidLeniencyValues = validValues;
        ValidLeniencyBounds = null;
    }
}