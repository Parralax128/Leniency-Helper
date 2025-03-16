using Monocle;
using static Celeste.Mod.LeniencyHelper.Tweaks.CustomBufferTime;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.UI;

public static class MenuButtonManager
{
    public static void LoadHooks()
    {
        On.Celeste.TextMenu.MoveSelection += RestrictMove;
    }
    public static void UnloadHooks()
    {
        On.Celeste.TextMenu.MoveSelection += RestrictMove;
    }
    private static void RestrictMove(On.Celeste.TextMenu.orig_MoveSelection orig, TextMenu self, int dir, bool wiggle)
    {
        if (self.Components.Get<LHmenuTracker>() is not null &&
            InSingleItemSuboptionsMenu && InSubOptionMode) return;

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
        var session = LeniencyHelperModule.Session;

        foreach (string tweak in session.TweaksEnabled.Keys)
        {
            SpecialSlider newTweak = new SpecialSlider(Dialog.Clean($"LENIENCYTWEAKS_{tweak.ToUpper()}"),
                tweak, SpecialSlider.GetIndexFromTweakName(tweak));
            newTweak.menu = menu;

            newTweak.OnValueChange += (index) =>
            {
                LeniencyHelperModule.Settings.TweaksByPlayer[tweak] = GetPlayerValueFromIndex(index);
                session.UpdateTweak(tweak);

                if (!session.TweaksEnabled[tweak]) newTweak.CloseDetailOptions();
            };
            
            menu.Add(newTweak);
        }
    }
    private static bool? GetPlayerValueFromIndex(int index)
    {
        switch (index)
        {
            case 0: return null;
            case 1: return true;
            case 2: return false;
            default: return null;
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

            if (InSubOptionMode)
            {
                foreach(TextMenu.Item item in thisMenu.Items)
                {
                    if (item is SpecialSlider slider && slider.addedDetailOptions)
                    {
                        slider.CloseDetailOptions();
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
        thisButton.OnPressed = () =>
        { 
            OnButtonPress(parentMenu);
        };

        return thisButton;
    }

}