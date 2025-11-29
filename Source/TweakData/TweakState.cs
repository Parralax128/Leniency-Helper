using Celeste.Mod.LeniencyHelper.TweakData.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper.TweakData;

public class TweakState
{
    public Tweak Tweak;

    
    private bool? _playerState = null;
    public bool? PlayerState
    {
        get => _playerState ;
        set
        {
            _playerState = value;
            CurrentSettingSource = UpdateSettingSource();
        }
    }
    
    private bool? _triggerState = false;
    public bool? TriggerState 
    {
        get => _triggerState ;
        set
        {
            _triggerState = value;
            CurrentSettingSource = UpdateSettingSource();
        }
    }
    
    private bool _controllerState = false;
    public bool ControllerState 
    {
        get => _controllerState ;
        set
        {
            _controllerState = value;
            CurrentSettingSource = UpdateSettingSource();
        }
    }
    
    private bool? _apiOverride = null;
    public bool? ApiOverride
    {
        get => _apiOverride;
        set
        {
            _apiOverride = value;
            CurrentSettingSource = UpdateSettingSource();
        }
    }

    private SettingSource UpdateSettingSource()
    {
        if (ApiOverride != null) return SettingSource.API;
        if (PlayerState != null) return SettingSource.Player;
        if (TriggerState != null) return SettingSource.Trigger;
        if (ControllerState == true) return SettingSource.Controller;
        return SettingSource.Default;
    }
    public SettingSource CurrentSettingSource = SettingSource.Default;

    public bool Enabled => ApiOverride ?? PlayerState ?? TriggerState ?? ControllerState;


    public SettingContainer Settings;
    private Dictionary<string, object> Temps;

    public TweakState(Tweak tweak, SettingContainer settings, Dictionary<string, object> temps = null)
    {
        Tweak = tweak;
        Settings = settings;
        Temps = temps;
    }

    public T GetSetting<T>(string key) => Settings.Get<T>(key, CurrentSettingSource);
    public T GetTemp<T>(string key) => (T)Temps[key];

    public static explicit operator Tweak(TweakState state) => state.Tweak;
    public static explicit operator string(TweakState state) => state.Tweak.ToString();
    public static explicit operator bool(TweakState state) => state.Enabled;
}