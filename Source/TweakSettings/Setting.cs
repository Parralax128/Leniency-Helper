using System.Collections.Generic;
using System;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;
public class Setting<T> : AbstractSetting
{
    public Dictionary<SettingSource, T> Values = new();
    public T Player
    {
        get => Values[SettingSource.Player];
        set => Values[SettingSource.Player] = value;
    }

    public Bounds<T> ValueBounds { get; private set; }

    private List<T> ValidLeniencyValues = null;
    private Bounds<T> ValidLeniencyBounds = null;

    public Setting(string name, T defaultValue)
    {
        Name = name;
        
        if(defaultValue is ICloneable clonable) foreach (var source in Enum.GetValues<SettingSource>())
                Values[source] = (T)clonable.Clone();
        
        else foreach (var source in Enum.GetValues<SettingSource>())
            Values[source] = defaultValue;
    }
    public Setting(string name, T defaultValue, T min, T max) : this(name, defaultValue)
    {
        ValueBounds = new Bounds<T>(min, max);
    }
    public Setting(string name, T defaultValue, T min) : this(name, defaultValue)
    {
        ValueBounds = new Bounds<T>(min);
    }

    public bool CheckLeniencyViolation(T value)
    {
        return ValidLeniencyBounds.Check(value);
    }
    public override bool? Set(SettingSource source, object value)
    {
        T typedValue = (T)value;

        bool? result = true;
        if (ValueBounds != null && !ValueBounds.Check(typedValue)) result = null;
        else if (source == SettingSource.Player && (ValidLeniencyBounds != null || ValidLeniencyValues != null))
        {
            result = ValidLeniencyBounds?.Check(typedValue) ?? ValidLeniencyValues.Contains(typedValue);
        }
        
        if(result == true)
        {
            Values[source] = typedValue;
        }
        
        return result;
    }
    public virtual T Get(SettingSource source)
    {
        return Values[source];
    }

    public override bool? Reset(SettingSource source)
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
    public override object ParseFromData(EntityData data)
    {
        T defaultValue = Get(SettingSource.Default);
        if (defaultValue is bool defaultBool) return data.Bool(Name, defaultBool);
        if (defaultValue is int defaultInt) return data.Int(Name, defaultInt);
        if (defaultValue is float defaultFloat) return data.Float(Name, defaultFloat);
        if (defaultValue is Time defaultTime) return data.Time(Name, defaultTime);

        return null;
    }

    public override List<UI.Items.AbstractTweakItem> MenuEntry(Tweak tweak)
    {
        return new() { new UI.Items.TweakSetting<T>(tweak, this) };
    }


    public T GetMapValue(Tweak tweak) => Values[tweak.GetMapSource()];
    public void CheckBounds(T value, out bool withMin, out bool withMax)
    {
        ValueBounds.Check(value, out withMin, out withMax);
    }

    public T this[SettingSource source]
    {
        get => Values[source];
        set => Set(source, value);
    }
}