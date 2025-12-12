using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.TweakSettings;
using Celeste.Mod.MaxHelpingHand.Module;
using IL.Monocle;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using static Celeste.GaussianBlur;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using static Celeste.Mod.LeniencyHelper.TweakExtension;
using static Celeste.TextMenu;

namespace Celeste.Mod.LeniencyHelper.UI.Items;

class TweakSlider : AbstractTweakItem
{
    public List<AbstractTweakItem> subSettings = new ();

    public bool addedSubsettings = false;
    Action<bool> SetVisible;

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

        cachedWidth = 0f;
        foreach (string str in Enum.GetValues<SliderValues>().Select(s => DialogUtils.Enum(s)))
        {
            cachedWidth = Math.Max(cachedWidth, ActiveFont.Measure(str).X);
        }

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

        var beforeOffset = new SubOptionsOffset(8);
        SetVisible += (v) => beforeOffset.Visible = v;

        Container.Add(beforeOffset);
        foreach (AbstractTweakItem setting in subSettings)
        {
            setting.Parent = this;
            if (setting.description != null) Container.Add(setting.description);
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

        var afterOffset = new SubOptionsOffset(16);
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

        if (Value == SliderValues.On) Container.Current = subSettings[0];
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
        Value = (SliderValues)((int)Value + dir);
        TweakData.Tweaks[tweak].Set(GetPlayerState, SettingSource.Player);

        ValueWiggler.Start();

        if (addedSubsettings) //if switched from "Map" to "ON" - updating suboptions via reopening
        {
            CloseSuboptions();
            OpenSuboptions();
        }

        if (!tweak.Enabled()) CloseSuboptions();
    }
    public override bool TryChangeValue(int dir)
    {
        return dir > 0 ? (int)Value < 3 : (int)Value > 0;
    }

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

    Color GetColor(bool selected)
    {
        if (selected) return Container.HighlightColor;

        if (tweak.Get(SettingSource.Player).HasValue) return PlayerValueColor;
        return tweak.Enabled() ? MapValueColor : UnselectedColor;
    }

    public override void Render(Vector2 position, bool selected)
    {
        position.X = Layout.LeftOffset;
        base.Render(position, selected, DialogUtils.Enum(Value), (int)Value > 0, (int)Value < 3, GetColor(selected) * Container.Alpha);
    }
}