using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;
class SettingContainer : IEnumerable<AbstractSetting>
{
    readonly List<AbstractSetting> settingList = new();

    public IEnumerator<AbstractSetting> GetEnumerator() => settingList.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => settingList.GetEnumerator();


    public void Add<T>(Setting<T> setting)
    {
        settingList.Add(setting);
    }

    AbstractSetting SettingByName(string name) => settingList.FirstOrDefault(s => s.Name == name);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get<T>(int index, SettingSource source) => (settingList[index] as Setting<T>).Get(source);
    public object Get(string name, SettingSource source) => SettingByName(name)?.GetTypeless(source);
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object Get(int index, SettingSource source) => settingList[index].GetTypeless(source);
    public void Set(int index, SettingSource source, object value) => settingList[index].Set(source, value);
    public void Set(string name, object value, SettingSource source = SettingSource.Player) => SettingByName(name)?.Set(source, value);
    
    public T Get<T>(string name, SettingSource source)
    {
        if (SettingByName(name) is Setting<T> setting) return setting.Get(source);

        throw new ArgumentException($"Invalid Setting<{typeof(T).Name}> name: {name}");
    }
}