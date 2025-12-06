using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using static Celeste.TextMenu;
using System.Diagnostics;
using Celeste.Mod.LeniencyHelper.TweakSettings;
using static Celeste.Mod.LeniencyHelper.TweakExtension;

namespace Celeste.Mod.LeniencyHelper.UI.Items;

public class TweakSlider : AbstractTweakItem
{
    public List<AbstractTweakItem> subSettings = new ();

    public bool addedSubsettings = false;
    private Action<bool> SetVisible;
    
    private SubOptionsOffset beforeSubOptionsSpacing;
    private SubOptionsOffset afterSubOptionsSpacing;

    private static readonly Color PlayerValueColor = new Color(218, 165, 32);
    private static readonly Color MapValueColor = new Color(0, 191, 255);
    public enum SliderValues
    {
        Map = 0, On = 1, Off = 2
    }

    public SliderValues Value;


    private bool? GetPlayerState => Value == SliderValues.Map ? null : Value == SliderValues.On;
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
        foreach (string str in Enum.GetValues<SliderValues>().Select(s => DialogUtils.Lookup(s)))
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
            OnLeave += () => { if (addedSubsettings) CloseSuboptions(); };
        }
    }
    private void SetupSubItems()
    {
        int baseindex = Container.IndexOf(this);

        foreach (AbstractTweakItem setting in subSettings)
        {
            if(setting.description != null) Container.Add(setting.description);
            Container.Add(setting);

            SetVisible += (v) =>
            {
                if (setting.description != null) setting.description.Visible = v;
                setting.Visible = v;
            };

            if(subSettings.Count != 1)
            {
                subSettings[0].OnLeave += () =>
                {
                    LoopMenuOnLeave(baseindex + 1, baseindex + subSettings.Count);
                };
            }
        }

        Container.Insert(baseindex + 1, new SubOptionsOffset(8));
        Container.Insert(baseindex + subSettings.Count + 2, new SubOptionsOffset(16));

        SetVisible(false);
    }
    

    #region SubOptions
    public override void ConfirmPressed()
    {
        if (!TweakMenuManager.InSubsettingsMode)
        {
            OpenSuboptions();
        }
    }
    public void OpenSuboptions()
    {
        if (addedSubsettings || subSettings.Count <= 0) return;

        bool disabled = Value != SliderValues.On;
        foreach (Item item in subSettings) item.Disabled = disabled;

        SetVisible(true);

        TweakMenuManager.InSingleSubsettingMenu = subSettings.Count == 1;
        if (Value != SliderValues.On) Container.Selection = Container.IndexOf(this) + 1;
        TweakMenuManager.InSubsettingsMode = true;
        addedSubsettings = true;
    }
    public void CloseSuboptions()
    {
        if (!addedSubsettings || subSettings.Count <= 0) return;

        SetVisible(false);

        TweakMenuManager.InSubsettingsMode = false;
        TweakMenuManager.InSingleSubsettingMenu = false;
        addedSubsettings = false;

        Container.Selection = Container.IndexOf(this);
    }

    private void LoopMenuOnLeave(int minIndex, int maxIndex)
    {
        if (Container.Selection > maxIndex) Container.Selection = minIndex;
        else if (Container.Selection < minIndex) Container.Selection = maxIndex;
    }

    protected bool SubSettingSelected
    {
        get
        {
            foreach (AbstractTweakItem item in subSettings)
                if (item.Selected) return true;
            return false;
        }
    }

    #endregion

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
        Value = Value.Advance(dir);
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
        return Value.CheckRange(dir);
    }

    public void TweakInfoFromLink()
    {
        string url = DialogUtils.TweakToUrl(tweak);
        
        if(LeniencyHelperModule.Settings.LinkOpeningMode 
            == LeniencyHelperSettings.UrlActions.OpenInBrowser)
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        else TextInput.SetClipboardText(url);

        SelectWiggler.Start();
        Audio.Play("event:/ui/main/rollover_up");
    }
    public override float RightWidth() => cachedWidth;
    public override float LeftWidth() => ActiveFont.Measure(Label).X * Layout.SubSettingScale;

    private Color GetColor(bool selected)
    {
        if (selected) return Container.HighlightColor;

        if (tweak.Get(SettingSource.Player).HasValue) return PlayerValueColor;
        return tweak.Enabled() ? MapValueColor : UnselectedColor;
    }

    public override void Render(Vector2 position, bool selected)
    {
        position.X = Layout.LeftOffset;
        base.Render(position, selected, DialogUtils.Lookup(Value),
            (int)Value > 0, (int)Value < 3, GetColor(selected) * Container.Alpha);
        RenderDescription = selected || (addedSubsettings && SubSettingSelected);
    }
}