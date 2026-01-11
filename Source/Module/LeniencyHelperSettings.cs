using System;
using System.Collections.Generic;
using System.Linq;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using Celeste.Mod.LeniencyHelper.TweakSettings;

namespace Celeste.Mod.LeniencyHelper.Module;

class LeniencyHelperSettings : EverestModuleSettings
{
    [SettingName("LENIENCYHELPER_SETTINGS_SHOWSETTINGS")]
    public bool showSettings { get; set; } = true;


    public enum UrlActions
    {
        OpenInBrowser,
        CopyToClipboard
    }

    [SettingName("LENIENCYHELPER_SETTINGS_LINKOPENINGMODE")]
    public UrlActions LinkOpeningMode { get; set; } = UrlActions.OpenInBrowser;


    static Tweak? FromString(string tweakName)
    {
        if (Enum.TryParse(tweakName, false, out Tweak result)) return result;
        return null;
    }

    
    public Dictionary<string, bool?> TweakStates
    {
        get
        {
            return TweakList.Select(tweak => new KeyValuePair<string, bool?>(tweak.ToString(), TweakData.Tweaks[tweak].Get(SettingSource.Player))).ToDictionary();
        }
        set
        {
            foreach(string tweakName in value.Keys)
            {
                Tweak? parsed = FromString(tweakName);
                if (parsed.HasValue) TweakData.Tweaks[parsed.Value].Set(value[tweakName], SettingSource.Player);
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
                        localDict.Add(setting.Name, boolSetting.Get(SettingSource.Player));
                    }
                }
                if (localDict.Count > 0) result.Add(tweak.ToString(), localDict);
            }

            return result;
        }

        set
        {
            foreach(string tweakName in value.Keys)
            {
                foreach (KeyValuePair<string, bool> pair in value[tweakName])
                {
                    Tweak? parsed = FromString(tweakName);
                    if(parsed.HasValue) TweakData.Tweaks[parsed.Value].Settings.Set(pair.Key, pair.Value);
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
                        localDict.Add(setting.Name, intSetting.Get(SettingSource.Player));
                    }
                }
                if (localDict.Count > 0) result.Add(tweak.ToString(), localDict);
            }

            return result;
        }

        set
        {
            foreach(string tweakName in value.Keys)
            {
                foreach (KeyValuePair<string, int> pair in value[tweakName])
                {
                    Tweak? parsed = FromString(tweakName);
                    if(parsed.HasValue) TweakData.Tweaks[parsed.Value].Settings.Set(pair.Key, pair.Value);
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
                        localDict.Add(setting.Name, timeSetting.Get(SettingSource.Player).Seconds);
                    }
                }
                if (localDict.Count > 0) result.Add(tweak.ToString(), localDict);
            }

            return result;
        }

        set
        {
            foreach(string tweakName in value.Keys)
            {
                foreach (KeyValuePair<string, float> pair in value[tweakName])
                {
                    Tweak? parsed = FromString(tweakName);
                    if (parsed.HasValue)
                    {
                        TweakData.Tweaks[parsed.Value].Settings.Get<Time>(pair.Key, SettingSource.Player).Seconds = pair.Value;
                    }
                }
            }
        }
    }
    
    public Dictionary<string, Dictionary<string, bool>> TimeModeSettings
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
                    if (setting is Setting<Time> timeSetting)
                    {
                        localDict.Add(setting.Name, timeSetting.Get(SettingSource.Player).Mode == Time.Modes.Frames);
                    }
                }
                if (localDict.Count > 0) result.Add(tweak.ToString(), localDict);
            }

            return result;
        }
        set
        {
            foreach(string tweakName in value.Keys)
            {
                foreach (KeyValuePair<string, bool> pair in value[tweakName])
                {
                    Tweak? parsed = FromString(tweakName);
                    if (parsed.HasValue)
                    {
                        TweakData.Tweaks[parsed.Value].Settings.Get<Time>(pair.Key, SettingSource.Player).Mode =
                            pair.Value ? Time.Modes.Frames : Time.Modes.Seconds;
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
                        localDict.Add(setting.Name, modeSetting.Get(SettingSource.Player));
                    }
                }
                if (localDict.Count > 0) result.Add(tweak.ToString(), localDict);
            }

            return result;
        }

        set
        {
            foreach(string tweakName in value.Keys)
            {
                foreach (KeyValuePair<string, FlexDistance.Modes> pair in value[tweakName])
                {
                    Tweak? parsed = FromString(tweakName);
                    if(parsed.HasValue) TweakData.Tweaks[parsed.Value].Settings.Set(pair.Key, pair.Value);
                }
            }
        }
    }

}