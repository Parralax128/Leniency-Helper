using System;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;

abstract class AbstractSetting
{
    public string Name;
    public abstract UI.Items.AbstractTweakItem MenuEntry(Tweak tweak);
    public abstract void Set(SettingSource source, object value);
    public abstract object GetTypeless(SettingSource source);
    public abstract void Reset(SettingSource source);

    public abstract object ParseFromData(EntityData data, Tweak tweak);
}