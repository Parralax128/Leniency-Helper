using Celeste.Mod.LeniencyHelper.TweakSettings;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.Module;

class LeniencyHelperSession : EverestModuleSession
{
    [OnLoad]
    public static void LoadHooks()
    {
        Everest.Events.Level.OnEnter += ResetSession;
        Everest.Events.Level.OnAfterUpdate += Silly;
    }

    static void Silly(Level level)
    {
        return;
        
        if(Input.Grab.Pressed)
        {
            foreach (Tweak tweak in TweakList)
            {
                TweakData.Tweaks[tweak].Set(null, SettingSource.Controller);

                if (!tweak.HasSettings()) continue;

                foreach (AbstractSetting setting in TweakData.Tweaks[tweak].Settings)
                {
                    setting.Reset(SettingSource.Controller);
                    Debug.Log($"reset {tweak}/{setting.Name} = {setting.GetTypeless(SettingSource.Controller)}");
                }
            }
        }
    }

    static void ResetSession(Session session, bool fromSaveData)
    {
        
    }

    public int wjDistR { get; set; } = 3;
    public int wjDistL { get; set; } = 3;




    static Tweak? FromString(string tweakName)
    {
        if (Enum.TryParse(tweakName, false, out Tweak result)) return result;
        return null;
    }


    public Dictionary<string, bool?> TweakStates
    {
        get
        {
            Debug.Warn("Tweak states getter!!");
            return TweakList.Select(tweak => new KeyValuePair<string, bool?>(tweak.ToString(), TweakData.Tweaks[tweak].Get(SettingSource.Controller))).ToDictionary();
        }
        set
        {
            Debug.Warn("Tweak states setter!");
            foreach (string tweakName in value.Keys)
            {
                Tweak? parsed = FromString(tweakName);
                if (parsed.HasValue) TweakData.Tweaks[parsed.Value].Set(value[tweakName], SettingSource.Controller);
            }
        }
    }


    public Dictionary<string, Dictionary<string, bool>> BoolSettings
    {
        get
        {
            Dictionary<string, Dictionary<string, bool>> result = new();
            foreach (Tweak tweak in TweakList)
            {
                if (!tweak.HasSettings()) continue;

                Dictionary<string, bool> localDict = new();
                foreach (AbstractSetting setting in TweakData.Tweaks[tweak].Settings)
                {
                    if (setting is Setting<bool> boolSetting)
                    {
                        localDict.Add(setting.Name, boolSetting.Get(SettingSource.Controller));
                    }
                }
                if (localDict.Count > 0) result.Add(tweak.ToString(), localDict);
            }
            return result;
        }

        set
        {
            foreach (string tweakName in value.Keys)
            {
                foreach (KeyValuePair<string, bool> pair in value[tweakName])
                {
                    Tweak? parsed = FromString(tweakName);
                    if (parsed.HasValue) TweakData.Tweaks[parsed.Value].Settings.Set(pair.Key, pair.Value, SettingSource.Controller);
                }
            }
        }
    }


    public Dictionary<string, Dictionary<string, int>> IntSettings
    {
        get
        {
            Dictionary<string, Dictionary<string, int>> result = new();
            foreach (Tweak tweak in TweakList)
            {
                if (!tweak.HasSettings()) continue;

                Dictionary<string, int> localDict = new();
                foreach (AbstractSetting setting in TweakData.Tweaks[tweak].Settings)
                {
                    if (setting is Setting<int> intSetting)
                    {
                        localDict.Add(setting.Name, intSetting.Get(SettingSource.Controller));
                    }
                }
                if (localDict.Count > 0) result.Add(tweak.ToString(), localDict);
            }

            return result;
        }

        set
        {
            foreach (string tweakName in value.Keys)
            {
                foreach (KeyValuePair<string, int> pair in value[tweakName])
                {
                    Tweak? parsed = FromString(tweakName);
                    if (parsed.HasValue) TweakData.Tweaks[parsed.Value].Settings.Set(pair.Key, pair.Value, SettingSource.Controller);
                }
            }
        }
    }


    public Dictionary<string, Dictionary<string, float>> TimeSettings
    {
        get
        {
            Dictionary<string, Dictionary<string, float>> result = new();
            foreach (Tweak tweak in TweakList)
            {
                if (!tweak.HasSettings()) continue;

                Dictionary<string, float> localDict = new();
                foreach (AbstractSetting setting in TweakData.Tweaks[tweak].Settings)
                {
                    if (setting is Setting<Time> timeSetting)
                    {
                        localDict.Add(setting.Name, timeSetting.Get(SettingSource.Controller).Seconds);
                    }
                }
                if (localDict.Count > 0) result.Add(tweak.ToString(), localDict);
            }

            return result;
        }

        set
        {
            foreach (string tweakName in value.Keys)
            {
                foreach (KeyValuePair<string, float> pair in value[tweakName])
                {
                    Tweak? parsed = FromString(tweakName);
                    if (parsed.HasValue)
                    {
                        TweakData.Tweaks[parsed.Value].Settings.Get<Time>(pair.Key, SettingSource.Controller).Seconds = pair.Value;
                    }
                }
            }
        }
    }

    public Dictionary<string, Dictionary<string, FlexDistance.Modes>> DistanceModeSettings
    {
        get
        {
            Dictionary<string, Dictionary<string, FlexDistance.Modes>> result = new();
            foreach (Tweak tweak in TweakList)
            {
                if (!tweak.HasSettings()) continue;

                Dictionary<string, FlexDistance.Modes> localDict = new();
                foreach (AbstractSetting setting in TweakData.Tweaks[tweak].Settings)
                {
                    if (setting is Setting<FlexDistance.Modes> modeSetting)
                    {
                        localDict.Add(setting.Name, modeSetting.Get(SettingSource.Controller));
                    }
                }
                if (localDict.Count > 0) result.Add(tweak.ToString(), localDict);
            }

            return result;
        }

        set
        {
            foreach (string tweakName in value.Keys)
            {
                foreach (KeyValuePair<string, FlexDistance.Modes> pair in value[tweakName])
                {
                    Tweak? parsed = FromString(tweakName);
                    if (parsed.HasValue) TweakData.Tweaks[parsed.Value].Settings.Set(pair.Key, pair.Value);
                }
            }
        }
    }
}