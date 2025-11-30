using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;

public class CompoundSetting<T> : AbstractSetting where T : class
{
    private SettingContainer subsettings;
    private Action<T, SettingContainer, SettingSource> valueUpdater;

    private T cachedValue;
    public T Get(SettingSource source)
    {
        valueUpdater.Invoke(cachedValue, subsettings, source);
        return cachedValue;
    }
    public CompoundSetting(string name, SettingContainer subsettings, Action<T, SettingContainer, SettingSource> updater)
    {
        Name = name;
        this.subsettings = subsettings;
        valueUpdater = updater;
    }

    public override bool? Set(SettingSource source, object value)
    {
        throw new InvalidOperationException("cannot directly set compound setting!");
    }
    public override bool? Reset(SettingSource source)
    {
        bool? result = true;
        bool? current;
        foreach(AbstractSetting setting in subsettings)
        {
            current = setting.Reset(source);
            if ((current == false && result != null) || current == null)
                result = current;
        }

        return result;
    }

    public override object ParseFromData(EntityData data)
    {
        Dictionary<string, object> result = new();
        foreach (AbstractSetting setting in subsettings)
        {
            result.Add(setting.Name, setting.ParseFromData(data));
        }
        return result;
    }

    public override List<TextMenu.Item> MenuEntry(Tweak tweak)
    {
        return subsettings.SelectMany(setting => setting.MenuEntry(tweak)).ToList();
    }
}