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

namespace Celeste.Mod.LeniencyHelper.UI;

public class TweakSlider : AbstractTweakItem
{
    public List<Item> subOptions = new List<Item>();

    public bool addedSubsettings = false;

    private float maxValueWidth = 0f;
    
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
        if (tweak.Get(TweakSettings.SettingSource.Player) != null)
            return tweak.Get(TweakSettings.SettingSource.Player) == true ? 1 : 2;
        else return 0;
    }
    public TweakSlider(Tweak tweak) : base(tweak)
    {
        this.tweak = tweak;

        foreach(string str in Enum.GetValues<SliderValues>().Select(s => DialogUtils.Lookup((object)s)))
        {
            maxValueWidth = Math.Max(maxValueWidth, ActiveFont.Measure(str).X);
        }
    }
    public override void Added()
    {
        base.Added();

        if (tweak.HasSettings())
        {
            AddSubOptions();
            beforeSubOptionsSpacing = new SubOptionsOffset(8);
            afterSubOptionsSpacing = new SubOptionsOffset(16);

            OnLeave += () => { if (addedSubsettings) CloseSuboptions(); };
        }
    }

    #region SubOptions
    private void AddSubOptions()
    {
        subOptions = TweakData.Tweaks[tweak].CreateMenuEntry();
    }
    public override void ConfirmPressed()
    {
        if (!TweakMenuManager.InSubsettingsMode)
        {
            OpenSuboptions();
        }
    }
    public void OpenSuboptions()
    {
        if (addedSubsettings || subOptions.Count <= 0) return;

        //not allowed to change options if not ON
        if (Value != SliderValues.On) foreach (Item item in subOptions) item.Disabled = true;
        else foreach (Item item in subOptions) item.Disabled = false;

        int optionsIndex = Container.Items.FindIndex(item => item.GetType() == typeof(TweakSlider) && ((TweakSlider)item).Label == Label);
        optionsIndex++;

        Container.Insert(optionsIndex - 1, beforeSubOptionsSpacing);

        int counter = 0;
        foreach (Item option in subOptions)
        {
            Container.Insert(optionsIndex + 1 + counter, option);
            Container.Selection = optionsIndex + 1 + counter++;

            if (subOptions.Count == 1) TweakMenuManager.InSingleSubsettingMenu = true;
            else
            {
                TweakMenuManager.InSingleSubsettingMenu = false;
                option.OnLeave += () =>
                {
                    LoopMenuOnLeave(optionsIndex + 1, optionsIndex + subOptions.Count);
                };
            }
        }
        if (Value != SliderValues.On) Container.Selection = optionsIndex;

        Container.Insert(optionsIndex + 1 + subOptions.Count, afterSubOptionsSpacing);

        TweakMenuManager.InSubsettingsMode = true;
        addedSubsettings = true;
    }
    public void CloseSuboptions()
    {
        if (!addedSubsettings || subOptions.Count <= 0) return;

        foreach (Item item in subOptions)
        {
            Container.Remove(item);
        }
        Container.Remove(beforeSubOptionsSpacing);
        Container.Remove(afterSubOptionsSpacing);

        TweakMenuManager.InSubsettingsMode = false;
        TweakMenuManager.InSingleSubsettingMenu = false;
        addedSubsettings = false;

        Container.Selection = Container.Items.FindIndex(item => item.Equals(this));
    }

    private void LoopMenuOnLeave(int minIndex, int maxIndex)
    {
        if (Container.Selection > maxIndex) Container.Selection = minIndex;
        else if (Container.Selection < minIndex) Container.Selection = maxIndex;
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

    public override float RightWidth()
    {
        return maxValueWidth;
    }
    private Color GetColor(bool selected)
    {
        if (selected) return Container.HighlightColor;

        if (tweak.Get(TweakSettings.SettingSource.Player).HasValue) return PlayerValueColor;
        return tweak.Enabled() ? MapValueColor : UnselectedColor;
    }

    public override void Render(Vector2 position, bool selected)
    {
        base.Render(position, selected, DialogUtils.Lookup(Value), (int)Value > 0, (int)Value < 3);
    }
}