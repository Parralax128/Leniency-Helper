using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;
public class SettingContainer : IEnumerable<AbstractSetting>
{
    private readonly List<AbstractSetting> settingList = new();

    public IEnumerator<AbstractSetting> GetEnumerator() => settingList.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => settingList.GetEnumerator();

    public void Add(AbstractSetting setting) => settingList.Add(setting);
    public void Add<T>(Setting<T> setting) => settingList.Add(setting);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get<T>(int index, SettingSource source)
    {
        if (settingList[index] is Setting<T> typeSetting) return typeSetting.Get(source);
        else return (settingList[index] as CompoundSetting<T>).Get(source);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object Get(int index, SettingSource source) => settingList[index].GetTypeless(source);

    // would need to fill up the Dictionary<string, int> (name -> setting index) for that
    // performance sucks, but ok for one-time assignments like triggers / controllers
    public bool? Set(int index, SettingSource source, object value) => settingList[index].Set(source, value);
}