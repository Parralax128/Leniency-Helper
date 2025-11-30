using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;
public class SettingContainer : IEnumerable<AbstractSetting>
{
    private readonly Dictionary<string, AbstractSetting> settingDict = new();

    public IEnumerator<AbstractSetting> GetEnumerator() => settingDict.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => settingDict.Values.GetEnumerator();


    public void Add(AbstractSetting setting) => settingDict.Add(setting.Name, setting);
    public void Add<T>(Setting<T> setting)
    {
        settingDict.Add(setting.Name, setting);
    }

    public AbstractSetting Get(string name, SettingSource source) => settingDict[name];
    public T Get<T>(string name, SettingSource source) => (settingDict[name] as Setting<T>).Get(source);
    public bool? Set(string name, SettingSource source, object value) => settingDict[name].Set(source, value);

    public AbstractSetting this[string name] => settingDict[name];
}