using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.UI.Items;
using Celeste.Mod.UI;
using Monocle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Celeste.Mod.LeniencyHelper.UI;

public static class TweakMenuManager
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.TextMenu.MoveSelection += RestrictMove;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.TextMenu.MoveSelection += RestrictMove;
    }


    public static MenuLayout Layout;
    static void RestrictMove(On.Celeste.TextMenu.orig_MoveSelection orig, TextMenu self, int dir, bool wiggle)
    {
        if (self.Components.Get<MenuTracker>() != null && InSingleSubsettingMenu && InSubsettingsMode)
        {
            return;
        }

        orig(self, dir, wiggle);
    }

    public static bool InSingleSubsettingMenu = false;
    public static bool InSubsettingsMode = false;
    static Action searchLabel;

    static TextMenu BuildMenu()
    {
        TextMenu menu = new TextMenu();

        Layout = new MenuLayout()
        {
            LeftOffset = 80f,
            RightOffset = 960f,
            SubSettingOffset = 100f,

            VideoSize = new Microsoft.Xna.Framework.Vector2(800f, 450f),
            VideoOffsetX = 100f,
            VideoPosY = 250f,

            TweakScale = 0.8f,
            SubSettingScale = 0.7f,

            DescVerticalOffset = 10f,
            DescriptionPos = DescriptionPos.UnderPlayer
        };
        Layout.Update();

        menu.OnUpdate += () => OnUpdate(menu);
        menu.Add(new TextMenu.Header(Dialog.Clean("MODOPTIONS_LENIENCYHELPER_MENU")));
        AddItemsToMenu(menu);
        menu.Add(new MenuTracker());
        
        searchLabel = OuiModOptions.AddSearchBox(menu);

        return menu;
    }

    static void AddItemsToMenu(TextMenu menu)
    {
        TextMenu.Button resetTweaksButton = new TextMenu.Button(Dialog.Clean("MODOPTIONS_LENIENCYHELPER_MENU_RESETTWEAKS"));
        resetTweaksButton.OnPressed = () =>
        {
            Audio.Play("event:/ui/main/button_toggle_off");
            resetTweaksButton.SelectWiggler.Start();
        };

        TextMenu.Button resetSettingsButton = new TextMenu.Button(Dialog.Clean("MODOPTIONS_LENIENCYHELPER_MENU_RESETSETTINGS"));
        resetSettingsButton.OnPressed = () =>
        {
            Audio.Play("event:/ui/main/button_toggle_off");
            resetSettingsButton.SelectWiggler.Start();
            TweakData.ResetPlayerSettings();
        };

        menu.Add(new TutorialPlayer());
        menu.Add(new HeightGap(32));
        menu.Add(new OptionsHint());

        foreach (Tweak tweak in LeniencyHelperModule.TweakList)
        {
            TweakSlider newTweak = new TweakSlider(tweak);
            if(newTweak.Description != null) menu.Add(newTweak.Description);

            resetTweaksButton.OnPressed += newTweak.Reset;
            menu.Add(newTweak);
        }
        menu.Insert(1, resetTweaksButton);
        menu.Insert(2, resetSettingsButton);
        menu.Selection = 1;
    }
    static void OnUpdate(TextMenu menu)
    {
        AbstractTweakItem selectedItem = menu.Items[menu.Selection] as AbstractTweakItem;

        if (Input.Grab.Pressed)
        {
            Input.Grab.ConsumeBuffer();

            if (selectedItem is TweakSlider tweakSlider)
            {
                tweakSlider.TweakInfoFromLink();
            }
            else
            {
                foreach (TweakSlider slider in menu.Items.FindAll(i => i is TweakSlider))
                {
                    if (slider.subSettings.Contains(selectedItem))
                    {
                        slider.TweakInfoFromLink();
                        break;
                    }
                }
            }
        }
        if(Input.QuickRestart.Pressed)
        {
            Input.QuickRestart.ConsumeBuffer();
            searchLabel.Invoke();
        }
    }
    static void OnButtonPress(TextMenu parentMenu)
    {
        LeniencyHelperModule.Instance.LoadSettings();

        Level level = Engine.Scene as Level;
        parentMenu.RemoveSelf();
        TextMenu thisMenu = BuildMenu();

        bool comesFromPauseMainMenu = level.PauseMainMenuOpen;
        level.PauseMainMenuOpen = false;

        thisMenu.OnCancel = () =>
        {
            Audio.Play(SFX.ui_main_button_back);
            LeniencyHelperModule.Instance.SaveSettings();

            if (InSubsettingsMode)
            {
                InSingleSubsettingMenu = false;
                foreach (TextMenu.Item item in thisMenu.Items)
                {
                    if (item is TweakSlider slider && slider.addedSubsettings)
                    {
                        slider.CloseSuboptions();
                        break;
                    }
                }
            }
            else
            {
                thisMenu.Close();
                level.Add(parentMenu);
                level.PauseMainMenuOpen = comesFromPauseMainMenu;

                level.AllowHudHide = true;
            }
        };

        thisMenu.OnESC = () =>
        {
            Audio.Play(SFX.ui_main_button_back);
            LeniencyHelperModule.Instance.SaveSettings();
            InSubsettingsMode = false;
            InSingleSubsettingMenu = false;

            thisMenu.Close();
            level.Add(parentMenu);
            level.PauseMainMenuOpen = comesFromPauseMainMenu;
            level.AllowHudHide = true;
        };  

        thisMenu.OnPause = () =>
        {
            LeniencyHelperModule.Instance.SaveSettings();

            Audio.Play(SFX.ui_main_button_back);
            thisMenu.Close();
            level.Paused = false;
            Engine.FreezeTimer = 0.15f;
            level.AllowHudHide = true;
        };

        level.Add(thisMenu);
    }
    public static TextMenu.Button BuildMenuButton(TextMenu parentMenu)
    {
        TextMenu.Button thisButton = new TextMenu.Button(Dialog.Clean("MODOPTIONS_LENIENCYHELPER_MENUBUTTON"));
        thisButton.OnPressed = () => OnButtonPress(parentMenu);
        return thisButton;
    }
}