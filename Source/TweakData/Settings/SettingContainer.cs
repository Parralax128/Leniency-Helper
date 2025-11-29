using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.TweakData.Settings;
public class SettingContainer : IEnumerable<object>
{
    private readonly Dictionary<string, object> settingDict = new();

    public IEnumerator<object> GetEnumerator() => settingDict.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => settingDict.Values.GetEnumerator();


    public void Add<T>(Setting<T> setting)
    {
        settingDict.Add(setting.Name, setting);
    }

    public T Get<T>(string name, SettingSource source) => (settingDict[name] as Setting<T>).Get(source);
    public bool? Set<T>(string name, SettingSource source, T value) => (settingDict[name] as Setting<T>).Set(source, value);
}