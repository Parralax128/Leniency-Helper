using Monocle;
using System;

namespace Celeste.Mod.LeniencyHelper.UI;

public static class MenuButtonManager
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

    private static void RestrictMove(On.Celeste.TextMenu.orig_MoveSelection orig, TextMenu self, int dir, bool wiggle)
    {
        if (self.Components.Get<LHmenuTracker>() != null && InSingleItemSuboptionsMenu && InSubOptionMode)
            return;

        orig(self, dir, wiggle);
    }

    public static bool InSingleItemSuboptionsMenu = false;

    public static bool InSubOptionMode = false;
    private static TextMenu BuildMenu()
    {
        TextMenu menu = new TextMenu();

        menu.Add(new TextMenu.Header(Dialog.Clean("MODOPTIONS_LENIENCYHELPER_MENU")));
        AddItemsToMenu(menu);
        menu.Add(new LHmenuTracker());

        return menu;
    }
    private static void AddItemsToMenu(TextMenu menu)
    {
        TextMenu.Button resetTweaksButton = new TextMenu.Button(Dialog.Clean("MODOPTIONS_LENIENCYHELPER_MENU_RESETTWEAKS"));
        resetTweaksButton.OnPressed = () => Audio.Play("event:/ui/main/button_toggle_off");
        
        TextMenu.Button resetSettingsButton = new TextMenu.Button(Dialog.Clean("MODOPTIONS_LENIENCYHELPER_MENU_RESETSETTINGS"));
        resetSettingsButton.OnPressed = () =>
        {
            Audio.Play("event:/ui/main/button_toggle_off");
            SettingMaster.ResetPlayerSettings();
        };

        menu.Add(new SubOptionsOffset(32));

        foreach (string tweak in LeniencyHelperModule.tweakList)
        {
            SpecialSlider newTweak = new SpecialSlider(Dialog.Clean($"LENIENCYTWEAKS_{tweak.ToUpper()}"),
                tweak, SpecialSlider.GetIndexFromTweakName(tweak));
            newTweak.menu = menu;

            resetTweaksButton.OnPressed += () => { while (newTweak.Index > 0) newTweak.ChangeValue(-1); };
            resetSettingsButton.OnPressed += () => newTweak.UpdateSubsettings();

            menu.Add(newTweak);
        }
        menu.Insert(1, resetTweaksButton);
        menu.Insert(2, resetSettingsButton);
        menu.Selection = 1;
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

            if (InSubOptionMode)
            {
                foreach(TextMenu.Item item in thisMenu.Items)
                {
                    if (item is SpecialSlider slider && slider.addedSuboptions)
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
            }
        };

        thisMenu.OnESC = () => 
        {
            Audio.Play(SFX.ui_main_button_back);
            LeniencyHelperModule.Instance.SaveSettings();

            thisMenu.Close();
            level.Add(parentMenu);
            level.PauseMainMenuOpen = comesFromPauseMainMenu;
        };

        thisMenu.OnPause = () => 
        {
            LeniencyHelperModule.Instance.SaveSettings();

            Audio.Play(SFX.ui_main_button_back);
            thisMenu.Close();
            level.Paused = false;
            Engine.FreezeTimer = 0.15f;
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