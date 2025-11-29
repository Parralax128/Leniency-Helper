using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using static Celeste.TextMenu;
using System.Diagnostics;
using Celeste.Mod.LeniencyHelper.UI.CustomOptions;

namespace Celeste.Mod.LeniencyHelper.UI;

public class TweakSlider : TweakSetting<int>
{
    public List<Item> subOptions = new List<Item>();
    public bool addedSubsettings = false;

    private float maxValueWidth = 0f;
    
    private SubOptionsOffset beforeSubOptionsSpacing;
    private SubOptionsOffset afterSubOptionsSpacing;

    public Tweak tweak;

    private static readonly Color PlayerValueColor = new Color(218, 165, 32);
    private static readonly Color MapValueColor = new Color(0, 191, 255);
    public enum SliderValues
    {
        Map,
        On,
        Off
    }
    private static string GetDialogEnumValue(SliderValues enumValue) =>
        Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_ENUMVALUES_{enumValue.ToString().ToUpper()}");
    public static int GetIndexFromTweakName(Tweak tweak)
    {
        if (LeniencyHelperModule.Settings.PlayerTweaks[tweak] is not null)
            return LeniencyHelperModule.Settings.PlayerTweaks[tweak] == true ? 1 : 2;
        else return 0;
    }
    public TweakSlider(string label, Tweak tweak, int defaultIndex)
        : base(tweak, null, true, true)
    {
        this.tweak = tweak;
        
        Add(GetDialogEnumValue(SliderValues.Map), 0);
        Add(GetDialogEnumValue(SliderValues.On), 1);
        Add(GetDialogEnumValue(SliderValues.Off), 2);

        foreach (Tuple<string, int> value in Values)
        {
            maxValueWidth = Math.Max(ActiveFont.Measure(value.Item1).X, maxValueWidth);
        }


        Index = PreviousIndex = defaultIndex;
    }
    public override void Added()
    {
        base.Added();

        if (SettingMaster.AssociatedSettings[tweak] != null)
            AddSubOptions();

        if (subOptions.Count > 0)
        {
            beforeSubOptionsSpacing = new SubOptionsOffset(8);
            afterSubOptionsSpacing = new SubOptionsOffset(16);
        }

        OnLeave += () =>
        {
            if (addedSubsettings)
            {
                CloseSuboptions();
            }
        };
    }

    #region SubOptions
    private void AddSubOptions()
    {
        foreach (string setting in SettingMaster.AssociatedSettings[tweak])
        {
            SetupSubOption(setting, DefaultSettings[setting].GetType());
        }

        BoolSlider toggler = subOptions.Find(i => i.GetType() == typeof(BoolSlider) && (i as BoolSlider).framesModeToggler == true) as BoolSlider;
        if (toggler == null) return;

        toggler.OnValueChange += (value) =>
        {
            foreach (FloatSlider slider in subOptions.FindAll(slider => slider.GetType() == typeof(FloatSlider) && (slider as FloatSlider).isTimer).ToList())
            {
                slider.SwapMode(value);
                slider.ChangedValue();
            }
        };

        foreach (FloatSlider slider in subOptions.FindAll(slider =>
            slider.GetType() == typeof(FloatSlider)).ToList())
        {
            slider.transitionIntoFrames = toggler.value;
        }
    }

    private void SetupSubOption(string nameInSettings, Type type)
    {
        var settings = LeniencyHelperModule.Settings;

        Item newOption;

        if (type == typeof(bool))
        {
            newOption = new BoolSlider(nameInSettings.ToLower().Contains("inframes"), tweak, nameInSettings);
            OnValueChange += (value) => (newOption as BoolSlider).value = SettingMaster.GetSetting<bool>(nameInSettings, tweak);
        }
        else if (type == typeof(float))
        {
            newOption = new FloatSlider(0f, GetMaxFromName(nameInSettings), 2, tweak, nameInSettings);

            (newOption as FloatSlider).isTimer = IsTimer(nameInSettings);

            OnValueChange += (value) => (newOption as FloatSlider).value = SettingMaster.GetSetting<float>(nameInSettings, tweak);
        }
        else if (type == typeof(int))
        {
            newOption = new IntSlider(0, (int)GetMaxFromName(nameInSettings), tweak, nameInSettings);

            OnValueChange += (value) => (newOption as IntSlider).value = SettingMaster.GetSetting<int>(nameInSettings, tweak);
        }
        else if (type == typeof(Dirs))
        {
            newOption = new DirSlider(tweak, nameInSettings);
            OnValueChange += (value) => (newOption as DirSlider).value = SettingMaster.GetSetting<Dirs>(nameInSettings, tweak);
        }
        else return;

        subOptions.Add(newOption);
    }
    public float GetMaxFromName(string name)
    {
        switch (name)
        {
            case "DirectionalBufferTime": return 0.5f;
            case "staticSnapdownDistance": return 32f;
            case "iceWJLeniency": return 16f;
            case "newWallboosterAcceleration": return 100f;
            case "RefillCoyoteTime": return 0.085f;
            case "wallApproachTime": return 0.25f;
            case "wallCoyoteTime": return 0.25f;
        }
        if (name.Contains("Timing")) return 0.25f;
        if (name.ToLower().Contains("distance")) return 8;

        return 0.5f;
    }
    private bool IsTimer(string paramName)
    {
        return paramName.ToLower().Contains("time") || paramName.ToLower().Contains("timing")
            || paramName.ToLower().Contains("delay");
    }
    public void UpdateSubsettings()
    {
        foreach (BoolSlider option in subOptions.FindAll(item => item is BoolSlider))
            option.value = SettingMaster.GetSetting<bool>(option.settingName, tweak);

        foreach (FloatSlider option in subOptions.FindAll(item => item is FloatSlider))
            option.value = SettingMaster.GetSetting<float>(option.settingName, tweak);

        foreach (IntSlider option in subOptions.FindAll(item => item is IntSlider))
            option.value = SettingMaster.GetSetting<int>(option.settingName, tweak);

        foreach (DirSlider option in subOptions.FindAll(item => item is DirSlider))
            option.value = SettingMaster.GetSetting<Dirs>(option.settingName, tweak);
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

        //not allowed to change options if "Map" is selected
        if (Index != 1) foreach (Item item in subOptions) item.Disabled = true;
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
        if (Index != 1) Container.Selection = optionsIndex;

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
        ChangeValue(-1);
    }
    public override void RightPressed()
    {
        Audio.Play("event:/ui/main/button_toggle_on");
        ChangeValue(1);
    }
    public void ChangeValue(int dir)
    {
        if (dir == -1 && Index > 0 || dir == 1 && Index < Values.Count - 1)
        {
            PreviousIndex = Index;
            Index += dir;
            lastDir = dir;

            SettingMaster.SetPlayerTweak(tweak, Index == 0 ? null : Index == 1 ? true : false);
            if (OnValueChange != null) OnValueChange(Index);
        }
        ValueWiggler.Start();

        if (addedSubsettings) //if switched from "Map" to "ON" - updating suboptions via reopening
        {
            CloseSuboptions();
            OpenSuboptions();
        }

        if (!SettingMaster.GetTweakEnabled(tweak)) CloseSuboptions();
    }

    public static string TweakToUrl(Tweak tweak)
    {
        return "https://github.com/Parralax128/Leniency-Helper/wiki/" +
            ToWikiPageName(Dialog.Clean("LENIENCYTWEAKS_" + tweak.ToString().ToUpper(), Dialog.Languages["english"]));
    }
    private static string ToWikiPageName(string tweakNameUpper)
    {
        string result = "";

        for (int c = 0; c < tweakNameUpper.Length; c++)
        {
            if (tweakNameUpper[c] == ' ') result += '-';
            else if (tweakNameUpper[c] == '-') result += "%E2%80%90";
            else if (c >= 1 && tweakNameUpper[c - 1] != ' ') result += tweakNameUpper[c].ToString().ToLower();
            else result += tweakNameUpper[c];
        }

        return result;
    }

    public void TweakInfoFromLink()
    {
        string url = TweakToUrl(tweak);
        
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

        if (LeniencyHelperModule.Settings.PlayerTweaks[tweak].HasValue) return PlayerValueColor;
        return SettingMaster.GetTweakEnabled(tweak) ? MapValueColor : UnselectedColor;
    }
    public override void Render(Vector2 position, bool selected)
    {
        OverrideMainColor = GetColor(selected);
        BaseRender(Values[Index].Item1, position, selected, Index > 0, Index < Values.Count - 1);
    }
}