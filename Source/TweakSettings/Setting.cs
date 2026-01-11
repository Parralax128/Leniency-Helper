using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;
class Setting<T> : AbstractSetting
{
    public T[] Values = new T[5];

    string lookupName;
    public ValueTuple<string, object>? DisableOn;
    public T Player
    {
        get => Values[(int)SettingSource.Player];
        set => Values[(int)SettingSource.Player] = value;
    }

    public Bounds<T> ValueBounds;

    public Setting(string name, T defaultValue, ValueTuple<string, object>? disableOn = null)
    {
        Name = name;
        DisableOn = disableOn;

        if (defaultValue is ICloneable clonable) foreach (var source in Enum.GetValues<SettingSource>())
            Values[(int)source] = (T)clonable.Clone();
        
        else foreach (var source in Enum.GetValues<SettingSource>())
            Values[(int)source] = defaultValue;
    }
    public Setting(string name, T defaultValue, T min, T max, ValueTuple<string, object>? disableOn = null)
        : this(name, defaultValue, disableOn)
    {
        ValueBounds = new Bounds<T>(min, max);
    }
    public override void Set(SettingSource source, object value)
    {
        T typedValue = (T)value;
       
        if(ValueBounds != null && !ValueBounds.Check(typedValue))
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

    public override object ParseFromData(EntityData data, Tweak tweak)
    {
        lookupName ??= DialogUtils.Setting(Name, tweak, DialogUtils.Precision.ImmutableKey);
        object defaultValue = GetTypeless(SettingSource.Default);


        if (defaultValue is bool defaultBool)    return data.Bool(lookupName, defaultBool);
        if (defaultValue is int defaultInt)      return data.Int(lookupName, defaultInt);
        if (defaultValue is float defaultFloat)  return data.Float(lookupName, defaultFloat);
        if (defaultValue is Time defaultTime)    return data.Time(lookupName, defaultTime);

        return null;
    }

    public override UI.Items.AbstractTweakItem MenuEntry(Tweak tweak) => new UI.Items.TweakSetting<T>(tweak, this);
}