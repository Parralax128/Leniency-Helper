using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.TweakSettings;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Celeste.TextMenu;
using Celeste.Mod.LeniencyHelper.UI.Items.SettingHandlers;

namespace Celeste.Mod.LeniencyHelper.UI.Items;

class TweakSlider : AbstractTweakItem
{
    public List<AbstractTweakItem> subSettings = new ();

    public bool addedSubsettings = false;
    Action<bool> SetVisible;
    EnumHandler<SliderValues> handler = new();

    static readonly Color PlayerValueColor = new Color(218, 165, 32);
    static readonly Color MapValueColor = new Color(0, 191, 255);
    public enum SliderValues
    {
        Map = 0, On = 1, Off = 2
    }

    public SliderValues Value;
    


    bool? GetPlayerState => Value == SliderValues.Map ? null : Value == SliderValues.On;
    public static int GetIndexFromTweakName(Tweak tweak)
    {
        if (tweak.Get(SettingSource.Player) != null)
            return tweak.Get(SettingSource.Player) == true ? 1 : 2;
        else return 0;
    }
    public TweakSlider(Tweak tweak) : base(tweak)
    {
        this.tweak = tweak;

        cachedWidth = handler.CalculateMaxWidth(null);

        Value = tweak.Get(SettingSource.Player) switch
        {
            true => SliderValues.On,
            false => SliderValues.Off,
            null => SliderValues.Map
        };

        TextScale = Layout.TweakScale;
    }
    public override void Added()
    {
        base.Added();

        if (tweak.HasSettings())
        {
            subSettings = TweakData.Tweaks[tweak].CreateMenuEntry();
            SetupSubItems();
        }
    }
    void SetupSubItems()
    {
        int baseindex = Container.IndexOf(this);

        var beforeOffset = new HeightGap(8);
        SetVisible += (v) => beforeOffset.Visible = v;

        Container.Add(beforeOffset);
        foreach (AbstractTweakItem setting in subSettings)
        {
            if (setting.Description != null)
            {
                Container.Add(setting.Description);
                SetVisible += (v) => { if (v == false) setting.Description.Visible = v; };
            }

            setting.Parent = this;
            Container.Add(setting);

            SetVisible += (v) => { setting.Visible = v; };
        }
            
        if(subSettings.Count > 1)
        {
            int minIndex = Container.IndexOf(subSettings[0]);
            int maxIndex = Container.IndexOf(subSettings.Last());

            // looping subsettings
            subSettings[0].OnLeave += () => 
            {
                if (Container.Selection < Container.IndexOf(subSettings[0]))
                    Container.Current = subSettings.Last();
            };
            subSettings.Last().OnLeave += () =>
            {
                if (Container.Selection > Container.IndexOf(subSettings.Last()))
                    Container.Current = subSettings[0];
            };
        }

        var afterOffset = new HeightGap(16);
        SetVisible += (v) => afterOffset.Visible = v;

        Container.Add(afterOffset);

        SetVisible(false);
    }

    public override void ConfirmPressed()
    {
        if (!TweakMenuManager.InSubsettingsMode) OpenSuboptions();
        else CloseSuboptions();
    }

    public void OpenSuboptions()
    {
        if (addedSubsettings || subSettings.Count <= 0) return;

        bool disabled = Value != SliderValues.On;
        foreach (Item item in subSettings) item.Disabled = disabled;

        SetVisible(true);

        TweakMenuManager.InSingleSubsettingMenu = subSettings.Count == 1;
        TweakMenuManager.InSubsettingsMode = true;
        addedSubsettings = true;
        Container.SceneAs<Level>().AllowHudHide = false;

        if (Value == SliderValues.On)
        {
            Container.Current = subSettings[0];
            subSettings[0].SetDescriptionVisible(true);
        }
    }
    public void CloseSuboptions()
    {
        if (!addedSubsettings || subSettings.Count <= 0) return;

        SetVisible(false);

        TweakMenuManager.InSubsettingsMode = false;
        TweakMenuManager.InSingleSubsettingMenu = false;
        addedSubsettings = false;
        Container.SceneAs<Level>().AllowHudHide = true;

        Container.Current = this;
    }


    public override void LeftPressed()
    {
        Audio.Play("event:/ui/main/button_toggle_off");
        base.LeftPressed();
    }
    public override void RightPressed()
    {
        Audio.Play("event:/ui/main/button_toggle_on");
        base.RightPressed();
    }


    public override void ChangeValue(int dir)
    {
        Value = handler.Advance(Value, dir);
        TweakData.Tweaks[tweak].Set(GetPlayerState, SettingSource.Player);

        ValueWiggler.Start();

        if (addedSubsettings) //updating suboptions via reopening
        {
            CloseSuboptions();
            OpenSuboptions();
        }

        if (!tweak.Enabled()) CloseSuboptions();
    }
    public override bool TryChangeValue(int dir) => handler.CheckValidDir(Value, null, dir);

    public void TweakInfoFromLink()
    {
        string url = DialogUtils.TweakToUrl(tweak);
        
        if(LeniencyHelperModule.Settings.LinkOpeningMode == LeniencyHelperSettings.UrlActions.OpenInBrowser)
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        else TextInput.SetClipboardText(url);

        SelectWiggler.Start();
        Audio.Play("event:/ui/main/rollover_up");
    }
    public override float RightWidth() => cachedWidth;
    public override float LeftWidth() => ActiveFont.Measure(Label).X * Layout.SubSettingScale;

    public override void Render(Vector2 position, bool selected)
    {
        position.X = Layout.LeftOffset;

        handler.CheckBounds(Value, out bool left, out bool right);
        base.Render(position, selected, handler.GetText(Value), left, right, GetColor() * Container.Alpha);

        Color GetColor()
        {
            if (selected) return Container.HighlightColor;

            if (tweak.Get(SettingSource.Player).HasValue) return PlayerValueColor;
            return tweak.Enabled() ? MapValueColor : UnselectedColor;
        }
    }
}