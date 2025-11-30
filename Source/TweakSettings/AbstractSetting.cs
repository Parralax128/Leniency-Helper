using System;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;

public abstract class AbstractSetting
{
    public string Name;
    public abstract List<TextMenu.Item> MenuEntry(Tweak tweak);
    public abstract bool? Set(SettingSource source, object value);
    public abstract bool? Reset(SettingSource source);

    public abstract object ParseFromData(EntityData data);
}