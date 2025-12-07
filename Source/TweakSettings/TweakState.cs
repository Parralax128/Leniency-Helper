using Celeste.Mod.LeniencyHelper.TweakSettings;
using MonoMod;
using MonoMod.Core.Platforms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;

public class TweakState : IEnumerable<AbstractSetting>
{
    public Tweak Tweak;

    private bool?[] Values = 
    {
         null,  // Player = 0
         null,  // API = 1
         null,  // Trigger = 2
         false  // Controller = 3
    };

    public SettingSource CurrentSettingSource = SettingSource.Default;
    public bool Enabled = false;

    public bool? Get(SettingSource source) => Values[(int)source];
    public void Set(bool? value, SettingSource source)
    {
        Values[(int)source] = value;
        UpdateSettingSource();
        UpdateEnabled();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateSettingSource()
    {
        if (Values[(int)SettingSource.Player] != null) { CurrentSettingSource = SettingSource.Player; return; }
        if (Values[(int)SettingSource.API] != null) { CurrentSettingSource = SettingSource.API; return; }
        if (Values[(int)SettingSource.Trigger] != null) { CurrentSettingSource = SettingSource.Trigger; return; }
        if (Values[(int)SettingSource.Controller] == true) { CurrentSettingSource = SettingSource.Controller; return; }
        CurrentSettingSource = SettingSource.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateEnabled() => Enabled =
        Values[(int)SettingSource.Player]
        ?? Values[(int)SettingSource.API]
        ?? Values[(int)SettingSource.Trigger]
        ?? Values[(int)SettingSource.Controller] == true;



    public SettingContainer Settings;
    private Dictionary<string, object> Temps;

    public TweakState(Tweak tweak, SettingContainer settings = null, List<string> temps = null)
    {
        Tweak = tweak;
        Settings = settings;

        if (temps != null)
        {
            Temps = new();
            foreach (string s in temps)
                Temps.Add(s, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetSetting<T>(int index) => Settings.Get<T>(index, CurrentSettingSource);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetTemp<T>(string key) => (T)(Temps[key] ?? default(T));


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTemp(string key, object value) => Temps[key] = value;

    public static implicit operator string(TweakState state) => state.Tweak.ToString();
    public static implicit operator bool(TweakState state) => state.Enabled;

    public IEnumerator<AbstractSetting> GetEnumerator() => Settings.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Settings.GetEnumerator();

    public List<UI.Items.AbstractTweakItem> CreateMenuEntry()
    {
        return Settings?.SelectMany(setting => setting.MenuEntry(Tweak)).ToList();
    }
}