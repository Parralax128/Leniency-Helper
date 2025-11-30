using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.LeniencyHelper.TweakSettings;
using System.Collections.Generic;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.UI;

public class TweakSetting<T> : AbstractTweakItem
{
    protected Setting<T> Setting;
    public bool PlayerSource = false;
    private Func<T, string> ValueToText = (T value) =>
    {
        if (value is not Enum) return value.ToString();
        return DialogUtils.Lookup(value);
    };

    protected T DefaultValue { get; private set; }

    public TweakSetting(Tweak tweak, Setting<T> setting) : base(tweak, setting.Name)
    {
        this.Setting = setting;
        DefaultValue = setting.Get(SettingSource.Default);
        if (WebScrapper.TweaksInfo.ContainsKey(tweak))
        {
            description = new Description(() => Container.Alpha, WebScrapper.TweaksInfo[tweak], DialogUtils.Lookup(tweak, setting.Name));
        }
    }

    public override bool TryChangeValue(int dir)
    {
        return base.TryChangeValue(dir);
    }
    public override void ChangeValue(int dir)
    {
        if(Setting is Setting<float> floatSetting)
        {
            Setting.Set(SettingSource.Player, floatSetting.Get(SettingSource.Player) + dir * 0.01f);
        }
        if(Setting is Setting<int> intSetting)
        {
            Setting.Set(SettingSource.Player, intSetting.Get(SettingSource.Player) + dir);
        }
        if(Setting is Setting<bool> boolSetting)
        {
            Setting.Set(SettingSource.Player, !boolSetting.Get(SettingSource.Player));
        }

    }
    public override void ConfirmPressed()
    {
        Setting.Set(SettingSource.Player, DefaultValue);
    }

    public override void Update()
    {
        base.Update();

        if(Input.MenuJournal.Pressed && Setting is Setting<Time> timeSetting)
        {
            timeSetting.Get(SettingSource.Player).SwapMode();
        }
    }
    public override void Render(Vector2 position, bool selected)
    {
        T currentValue = Setting.Get(TweakData.Tweaks[tweak].CurrentSettingSource);
        Setting.CheckBounds(currentValue, out bool left, out bool right);

        base.Render(position, selected, ValueToText.Invoke(currentValue), left, right);
    }
}