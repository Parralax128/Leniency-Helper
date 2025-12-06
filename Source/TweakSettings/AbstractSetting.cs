using System;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.TweakSettings;

public abstract class AbstractSetting
{
    public string Name;
    public Func<bool> Visible = () => true;
    public abstract List<UI.Items.AbstractTweakItem> MenuEntry(Tweak tweak);
    public abstract bool? Set(SettingSource source, object value);
    public abstract bool? Reset(SettingSource source);

    public abstract object ParseFromData(EntityData data);
}