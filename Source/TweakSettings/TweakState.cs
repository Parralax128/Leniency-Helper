using Celeste.Mod.LeniencyHelper.TweakSettings;
using MonoMod;
using MonoMod.Core.Platforms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using static System.Reflection.CustomAttributeExtensions;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;

class TweakState : IEnumerable<AbstractSetting>
{
    public Tweak Tweak;

    bool?[] Values = 
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
    public List<object> Temps;

    public TweakState(Tweak tweak, SettingContainer settings = null)
    {
        Tweak = tweak;
        Settings = settings;

        if(settings != null) AssignTweakIndices();
    }

    void AssignTweakIndices()
    {
        Type tweakType = TweakData.TweakList.Types[Tweak];

        IEnumerable<FieldInfo> fields = tweakType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).
            Where(f => f.IsDefined(typeof(Tweaks.SettingIndexAttribute), false));

        if (fields == null || fields.Count() == 0) return;

        int counter = 0;
        foreach(AbstractSetting setting in Settings)
        {
            FieldInfo matchingField = fields.FirstOrDefault(f => (f.GetCustomAttribute<Tweaks.SettingIndexAttribute>(false).setting ?? f.Name) == setting.Name);

            matchingField?.SetValue(null, counter);
            counter++;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetSetting<T>(int index) => Settings.Get<T>(index, CurrentSettingSource);

    public static implicit operator string(TweakState state) => state.Tweak.ToString();
    public static implicit operator bool(TweakState state) => state.Enabled;

    public IEnumerator<AbstractSetting> GetEnumerator() => Settings.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Settings.GetEnumerator();

    public List<UI.Items.AbstractTweakItem> CreateMenuEntry()
    {
        return Settings?.Select(setting => setting.MenuEntry(Tweak)).ToList();
    }
}