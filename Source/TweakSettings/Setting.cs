using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;
class Setting<T> : AbstractSetting
{
    public T[] Values = new T[5];
    public T Player
    {
        get => Values[(int)SettingSource.Player];
        set => Values[(int)SettingSource.Player] = value;
    }

    public Bounds<T> ValueBounds { get; set; }

    List<T> ValidLeniencyValues = null;
    Bounds<T> ValidLeniencyBounds = null;

    public Setting(string name, T defaultValue)
    {
        Name = name;
        
        if(defaultValue is ICloneable clonable) foreach (var source in Enum.GetValues<SettingSource>())
            Values[(int)source] = (T)clonable.Clone();
        
        else foreach (var source in Enum.GetValues<SettingSource>())
            Values[(int)source] = defaultValue;
    }
    public Setting(string name, T defaultValue, T min, T max) : this(name, defaultValue)
    {
        ValueBounds = new Bounds<T>(min, max);
    }

    public bool CheckLeniencyViolation(T value)
    {
        return ValidLeniencyBounds.Check(value);
    }
    public override void Set(SettingSource source, object value)
    {
        T typedValue = (T)value;

        bool? check = true;
        if (ValueBounds != null && !ValueBounds.Check(typedValue)) check = null;
        else if (source == SettingSource.Player && (ValidLeniencyBounds != null || ValidLeniencyValues != null))
        {
            check = ValidLeniencyBounds?.Check(typedValue) ?? ValidLeniencyValues.Contains(typedValue);
        }
        
        if(check == true)
        {
            Values[(int)source] = typedValue;
        }
    }

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get(SettingSource source) => Values[(int)source];
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override object GetTypeless(SettingSource source) => Values[(int)source];


    public override void Reset(SettingSource source)
        => Set(source, Values[(int)SettingSource.Default]);


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

    public override UI.Items.AbstractTweakItem MenuEntry(Tweak tweak) =>  new UI.Items.TweakSetting<T>(tweak, this);
}