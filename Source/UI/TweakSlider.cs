using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.Tweaks;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using static Celeste.TextMenu;

namespace Celeste.Mod.LeniencyHelper.UI;

public class TweakSlider : Option<int>
{
    public TextMenu menu;

    public List<Item> subOptions = new List<Item>();
    public bool addedSubsettings = false;

    private SubOptionsOffset beforeSubOptions;
    private SubOptionsOffset afterSubOptions;

    public string tweakName;

    private static Color PlayerValueColor = new Color(218, 165, 32);
    private static Color MapValueColor = new Color(0, 191, 255);
    private static Color BothInactiveColor = Color.White;
    public enum SliderValues
    {
        Map,
        On,
        Off
    }
    private static string GetDialogEnumValue(SliderValues enumValue) =>
        Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_ENUMVALUES_{enumValue.ToString().ToUpper()}");
    public static int GetIndexFromTweakName(string tweakName)
    {
        if (LeniencyHelperModule.Settings.PlayerTweaks[tweakName] is not null)
            return (LeniencyHelperModule.Settings.PlayerTweaks[tweakName] == true ? 1 : 2);
        else return 0;
    }
    public TweakSlider(string label, string tweakName, int defaultIndex) : base(label)
    {
        this.tweakName = tweakName;

        Add(GetDialogEnumValue(SliderValues.Map), 0);
        Add(GetDialogEnumValue(SliderValues.On), 1);
        Add(GetDialogEnumValue(SliderValues.Off), 2);

        Index = PreviousIndex = defaultIndex;

        if (SettingMaster.AssociatedTweaks[tweakName] != null)
            AddSubOptions();

        if (subOptions.Count > 0)
        {
            beforeSubOptions = new SubOptionsOffset(8);
            afterSubOptions = new SubOptionsOffset(16);
        }

        OnLeave += () =>
        {
            if(addedSubsettings)
            {
                CloseSuboptions();
            }
        };
    }

    #region SubOptions
    private void AddSubOptions()
    {
        foreach (string setting in SettingMaster.AssociatedTweaks[tweakName])
        {
            SetupSubOption(setting, LeniencyHelperModule.DefaultSettings.Get(setting).GetType());
        }

        CustomOnOff toggler = subOptions.Find(i => i.GetType() == typeof(CustomOnOff) && (i as CustomOnOff).framesModeToggler == true) as CustomOnOff;
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
        string label = FromDialog(nameInSettings.ToLower().Contains("inframes")? "CountInFrames" : nameInSettings);
        var settings = LeniencyHelperModule.Settings;

        Item newOption;

        if (type == typeof(bool))
        {
            newOption = new CustomOnOff(label, SettingMaster.GetSetting<bool>(nameInSettings, tweakName),
                nameInSettings.ToLower().Contains("inframes"), nameInSettings);

            this.OnValueChange += (value) => (newOption as CustomOnOff).value = SettingMaster.GetSetting<bool>(nameInSettings, tweakName);
        }
        else if (type == typeof(float))
        {
            newOption = new FloatSlider(label, 0f, GetMaxFromName(nameInSettings),
                SettingMaster.GetSetting<float>(nameInSettings, tweakName), 2, nameInSettings);

            (newOption as FloatSlider).isTimer = IsTimer(nameInSettings);

            this.OnValueChange += (value) => (newOption as FloatSlider).value = SettingMaster.GetSetting<float>(nameInSettings, tweakName);


            if (tweakName == "CustomBufferTime")
            {
                this.OnValueChange += (value) =>
                {
                    CustomBufferTime.UpdateCustomBuffers();
                };
                (newOption as FloatSlider).OnValueChange += (value) =>
                {
                    CustomBufferTime.UpdateCustomBuffers();
                };
            }
        }
        else if (type == typeof(int))
        {
            newOption = new IntSlider(label, 0, (int)GetMaxFromName(nameInSettings),
                SettingMaster.GetSetting<int>(nameInSettings, tweakName), nameInSettings);

            this.OnValueChange += (value) =>
            {
                (newOption as IntSlider).value = SettingMaster.GetSetting<int>(nameInSettings, tweakName);
            };
        }
        else if(type == typeof(Dirs))
        {
            newOption = new DirSlider(label, SettingMaster.GetSetting<Dirs>(nameInSettings, tweakName), nameInSettings);

            this.OnValueChange += (value) =>
            {
                (newOption as DirSlider).value = SettingMaster.GetSetting<Dirs>(nameInSettings, tweakName);
            };
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

        return 0.5f;
    }

    public static string FromDialog(string str)
    {
        return ("     " + Dialog.Clean("MODOPTIONS_LENIENCYHELPER_SETTINGS_" + str.ToUpper()));
    }
    private bool IsTimer(string paramName)
    {
        return paramName.ToLower().Contains("time") || paramName.ToLower().Contains("timing");
    }
    public void UpdateSubsettings()
    {
        foreach (CustomOnOff option in subOptions.FindAll(item => item is CustomOnOff))
            option.value = SettingMaster.GetSetting<bool>(option.settingName, tweakName);
        
        foreach (FloatSlider option in subOptions.FindAll(item => item is FloatSlider))
            option.value = SettingMaster.GetSetting<float>(option.settingName, tweakName);
        
        foreach (IntSlider option in subOptions.FindAll(item => item is IntSlider))
            option.value = SettingMaster.GetSetting<int>(option.settingName, tweakName);
        
        foreach (DirSlider option in subOptions.FindAll(item => item is DirSlider))
            option.value = SettingMaster.GetSetting<Dirs>(option.settingName, tweakName);
    }


    public override void ConfirmPressed()
    {
        if (!MenuButtonManager.InSubsettingsMode)
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

        int optionsIndex = menu.Items.FindIndex(item => item.GetType() == typeof(TweakSlider) && ((TweakSlider)item).Label == Label);
        optionsIndex++;

        menu.Insert(optionsIndex-1, beforeSubOptions);

        foreach (Item option in subOptions)
        {
            menu.Insert(optionsIndex + 1, option);
            menu.Selection = optionsIndex + 1;

            if (subOptions.Count == 1) MenuButtonManager.InSingleSubsettingMenu = true;

            else
            {
                MenuButtonManager.InSingleSubsettingMenu = false;
                option.OnLeave += () =>
                {
                    LoopMenuOnLeave(optionsIndex + 1, optionsIndex + subOptions.Count);
                };
            }
        }
        if (Index != 1) menu.Selection = optionsIndex;

        menu.Insert(optionsIndex + 1 + subOptions.Count, afterSubOptions);

        MenuButtonManager.InSubsettingsMode = true;
        addedSubsettings = true;
    }
    public void CloseSuboptions()
    {
        if (!addedSubsettings || subOptions.Count <= 0) return;

        foreach (Item item in subOptions)
        {
            menu.Remove(item);
        }
        menu.Remove(beforeSubOptions);
        menu.Remove(afterSubOptions);

        MenuButtonManager.InSubsettingsMode = false;
        MenuButtonManager.InSingleSubsettingMenu = false;
        addedSubsettings = false;

        menu.Selection = menu.Items.FindIndex(item => item.Equals(this));
    }

    private void LoopMenuOnLeave(int minIndex, int maxIndex)
    {
        if (menu.Selection > maxIndex) menu.Selection = minIndex;
        else if(menu.Selection <  minIndex) menu.Selection = maxIndex;
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
        if ((dir == -1 && Index > 0) || (dir == 1 && Index < Values.Count - 1))
        {
            PreviousIndex = Index;
            Index += dir;
            lastDir = dir;

            SettingMaster.SetPlayerTweak(tweakName, Index == 0 ? null : (Index == 1 ? true : false));
            if (OnValueChange != null) OnValueChange(Index);
        }
        ValueWiggler.Start();

        if (addedSubsettings) //if switched from "Map" to "ON" - updating suboptions via reopening
        {
            CloseSuboptions();
            OpenSuboptions();
        }       

        if (!SettingMaster.GetTweakEnabled(tweakName)) CloseSuboptions();
    }
    

    public void CopyWikiLinkToCliboard()
    {
        TextInput.SetClipboardText("https://github.com/Parralax128/Leniency-Helper/wiki/" +
            ToWikiPageName(Dialog.Clean("LENIENCYTWEAKS_" + tweakName.ToUpper())));

        SelectWiggler.Start();
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

    private Color GetColor()
    {
        if (LeniencyHelperModule.Settings.PlayerTweaks[tweakName].HasValue) return PlayerValueColor;
        return SettingMaster.GetTweakEnabled(tweakName) ? MapValueColor : BothInactiveColor;
    }
    public override void Render(Vector2 position, bool highlighted)
    {
        float alpha = Container.Alpha;
        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        Color color = (Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : UnselectedColor) * alpha));

        if(!Disabled && !highlighted)
        {
            color = GetColor();
        }

        ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);
        if (Values.Count > 0)
        {
            float num = RightWidth();

            ActiveFont.DrawOutline(Values[Index].Item1, position + 
                new Vector2(Container.Width - num * 0.5f + (float)lastDir * ValueWiggler.Value * 8f, 0f),
                new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);

            Vector2 vector = Vector2.UnitX * (highlighted ? ((float)Math.Sin(sine * 4f) * 4f) : 0f);
            bool flag = Index > 0;
            Color color2 = (flag ? color : (Color.DarkSlateGray * alpha));

            Vector2 position2 = position + 
                new Vector2(Container.Width - num + 40f + ((lastDir < 0) ? ((0f - ValueWiggler.Value) * 8f) : 0f), 0f)
                - (flag ? vector : Vector2.Zero);

            ActiveFont.DrawOutline("<", position2, new Vector2(0.5f, 0.5f), Vector2.One, color2, 2f, strokeColor);


            bool flag2 = Index < Values.Count - 1;
            Color color3 = (flag2 ? color : (Color.DarkSlateGray * alpha));
            Vector2 position3 = position + new Vector2(Container.Width - 40f + 
                ((lastDir > 0) ? (ValueWiggler.Value * 8f) : 0f), 0f) + (flag2 ? vector : Vector2.Zero);

            ActiveFont.DrawOutline(">", position3, new Vector2(0.5f, 0.5f), Vector2.One, color3, 2f, strokeColor);
        }
    }
}