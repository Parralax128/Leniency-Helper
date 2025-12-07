using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.UI.Items;
using Celeste.Mod.UI;
using Monocle;
using System;
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
    private static void RestrictMove(On.Celeste.TextMenu.orig_MoveSelection orig, TextMenu self, int dir, bool wiggle)
    {
        if (self.Components.Get<LHmenuTracker>() != null && InSingleSubsettingMenu && InSubsettingsMode)
        {
            return;
        }

        if (self.Current is AbstractTweakItem customItem)
        {
            customItem.RenderDescription = false;
        }
        orig(self, dir, wiggle);
        if (self.Current is AbstractTweakItem newCustomItem)
        {
            newCustomItem.RenderDescription = true;
        }
    }

    public static bool InSingleSubsettingMenu = false;
    public static bool InSubsettingsMode = false;
    private static Action searchLabel;

    private static TextMenu BuildMenu()
    {
        TextMenu menu = new TextMenu();

        Layout = new MenuLayout();
        Layout.LeftOffset = 80f;
        Layout.RightOffset = 960f;
        Layout.SubSettingOffset = 100f;
        
        Layout.VideoSize = new Microsoft.Xna.Framework.Vector2(0.4f);
        Layout.VideoOffsetX = 0.1f;
        Layout.VideoPosY = 0.4f;

        Layout.TweakScale = 0.8f;
        Layout.SubSettingScale = 0.7f;

        menu.OnUpdate += () => OnUpdate(menu);
        menu.Add(new TextMenu.Header(Dialog.Clean("MODOPTIONS_LENIENCYHELPER_MENU")));
        AddItemsToMenu(menu);
        menu.Add(new LHmenuTracker());
        
        searchLabel = OuiModOptions.AddSearchBox(menu);

        return menu;
    }
    private static void AddItemsToMenu(TextMenu menu)
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
            SettingMaster.ResetPlayerSettings();
        };

        menu.Add(new SubOptionsOffset(32));
        menu.Add(new OptionsHint());

        foreach (Tweak tweak in LeniencyHelperModule.TweakList)
        {
            TweakSlider newTweak = new TweakSlider(tweak);
            if(newTweak.description != null) menu.Add(newTweak.description);

            resetTweaksButton.OnPressed += () => { while ((int)newTweak.Value > 0) newTweak.ChangeValue(-1); };
            menu.Add(newTweak);
        }
        menu.Insert(1, resetTweaksButton);
        menu.Insert(2, resetSettingsButton);
        menu.Selection = 1;       
    }
    private static void OnUpdate(TextMenu menu)
    {
        UI.Items.AbstractTweakItem selectedItem = menu.Items[menu.Selection] as AbstractTweakItem;

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
    private static void OnButtonPress(TextMenu parentMenu)
    {
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