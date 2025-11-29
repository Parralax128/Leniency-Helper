using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.UI;
using Monocle;
using System;
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
        if (self.Components.Get<LHmenuTracker>() != null)
        {
            if (InSingleSubsettingMenu && InSubsettingsMode) return;
            if (self.Items[self.Selection] is TweakSlider slider && slider.addedSubsettings) return;
        }

        orig(self, dir, wiggle);
    }

    public static bool InSingleSubsettingMenu = false;

    public static bool InSubsettingsMode = false;
    private static Action searchLabel;

    private static TextMenu BuildMenu()
    {
        TextMenu menu = new TextMenu();

        Layout = new MenuLayout();
        Layout.LeftOffset = 0.05f;
        Layout.RightOffset = 0.4f;
        Layout.SubSettingOffset = 0.03f;


        Layout.VideoSize = new Microsoft.Xna.Framework.Vector2(0.4f);
        Layout.VideoOffsetX = 0.1f;
        Layout.VideoPosY = 0.4f;


        Logging.Log($"menu size: {new Vector2(menu.Width, menu.Height)}");
        

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
            string tweakName = tweak.ToString();
            TweakSlider newTweak = new TweakSlider(Dialog.Clean($"LENIENCYTWEAKS_{tweakName.ToUpper()}"),
                tweak, TweakSlider.GetIndexFromTweakName(tweak));

            resetTweaksButton.OnPressed += () => { while (newTweak.Index > 0) newTweak.ChangeValue(-1); };
            resetSettingsButton.OnPressed += newTweak.UpdateSubsettings;

            menu.Add(newTweak);
        }
        menu.Insert(1, resetTweaksButton);
        menu.Insert(2, resetSettingsButton);
        menu.Selection = 1;

        TutorialPlayer.LoadVideo("vid");
        TutorialPlayer.PlayTutorial();
        
    }
    private static void OnUpdate(TextMenu menu)
    {
        TextMenu.Item selectedItem = menu.Items[menu.Selection];

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
                    if (slider.subOptions.Contains(selectedItem))
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
                InSubsettingsMode = false;
                InSingleSubsettingMenu = false;

                thisMenu.Close();
                level.Add(parentMenu);
                level.PauseMainMenuOpen = comesFromPauseMainMenu;
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