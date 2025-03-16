using System;
using System.Collections.Generic;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

public class GenericTweakTrigger : Triggers.GenericTrigger
{
    public struct TriggerData
    {
        public object value;
        public object defaultValue;
        public string nameInSettings;
        public string dataName;
        public string type;
        public TriggerData(object defaultValue, string name, string type)
        {
            value = defaultValue;
            this.defaultValue = defaultValue;

            nameInSettings = "map" + name[0].ToString().ToUpper() +
                name.Substring(1); ;

            dataName = name;

            this.type = type;
        }
        public TriggerData(object defaultValue, string settingName, string dataName, string type)
        {
            value = defaultValue;
            this.defaultValue = defaultValue;

            nameInSettings = "map" + settingName[0].ToString().ToUpper() +
                settingName.Substring(1); ;

            this.dataName = dataName;

            this.type = type;
        }
    }

    public TriggerData[] fullData = Array.Empty<TriggerData>();
    private Dictionary<string, object> oldValues = new Dictionary<string, object>();
    public string tweakName;

    private EntityData localData;

    public GenericTweakTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        localData = data;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);

        if (fullData.Length > 0)
        {
            for (int c = 0; c < fullData.Length; c++)
            {
                switch (fullData[c].type)
                {
                    case "bool":
                        fullData[c].value = localData.Bool(fullData[c].dataName, (bool)fullData[c].defaultValue);
                        break;
                    case "int":
                        fullData[c].value = localData.Int(fullData[c].dataName, (int)fullData[c].defaultValue);
                        break;
                    case "float":
                        fullData[c].value = localData.Float(fullData[c].dataName, (float)fullData[c].defaultValue);
                        break;
                    case "string":
                        fullData[c].value = localData.Attr(fullData[c].dataName, (string)fullData[c].defaultValue);
                        break;
                }
            }
        }
    }
    public override void ApplySettings()
    {
        LeniencyHelperModule.Session.SetMapValue(tweakName, enabled);

        if (fullData.Length > 0 && enabled)
        {
            var s = LeniencyHelperModule.Session;
            for (int c = 0; c < fullData.Length; c++)
            {
                if (!s.GetValue(fullData[c].nameInSettings).Equals(fullData[c].value))
                {
                    s.SetValue(fullData[c].nameInSettings, fullData[c].value);
                }
            }
        }
    }
    public override void GetOldSettings()
    {
        var s = LeniencyHelperModule.Session;
        
        oldValues.Clear();

        wasEnabled = s.TweaksByMap.ContainsKey(tweakName) ? s.TweaksByMap[tweakName] : false;
        if (fullData.Length > 0)
        {
            foreach (TriggerData data in fullData)
                oldValues.Add(data.nameInSettings, s.GetValue(data.nameInSettings));
        }            
    }
    public override void UndoSettings()
    {
        var s = LeniencyHelperModule.Session;

        s.SetMapValue(tweakName, wasEnabled);

        if (oldValues.Count > 0)
        {
            foreach (string name in oldValues.Keys)
                s.SetValue(name, oldValues[name]);

            oldValues.Clear();
        }
    }
}