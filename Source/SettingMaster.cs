using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using Celeste.Mod.LeniencyHelper.Triggers;
using System.Linq;
using System.Reflection;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper;
public static class SettingMaster
{
    #region tweaks
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.ctor += DisableTweaksOnRespawn;
        On.Celeste.LevelLoader.ctor += DisableTweaksOnLoad;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.ctor -= DisableTweaksOnRespawn;
        On.Celeste.LevelLoader.ctor -= DisableTweaksOnLoad;
    }

    public static bool GetTweakEnabled(string tweak)
    {
        if (LeniencyHelperModule.Settings.PlayerTweaks[tweak].HasValue)
            return LeniencyHelperModule.Settings.PlayerTweaks[tweak].Value;

        return LeniencyHelperModule.Session.UseController[tweak] ?
            LeniencyHelperModule.Session.ControllerTweaks[tweak] : LeniencyHelperModule.Session.TriggerTweaks[tweak];
    }
    public static void SetPlayerTweak(string tweak, bool? newValue)
    {
        LeniencyHelperModule.Settings.PlayerTweaks[tweak] = newValue;
    }
    public static void SetTriggerTweak(string tweak, bool newValue) => LeniencyHelperModule.Session.TriggerTweaks[tweak] = newValue;
    public static void SetControllerTweak(string tweak, bool newValue)
    {
        LeniencyHelperModule.Session.ControllerTweaks[tweak] = newValue;
    }
    public static void SetUseController(string tweak, bool useController) => LeniencyHelperModule.Session.UseController[tweak] = useController;

    public static void DisableTweaksOnRespawn(On.Celeste.Player.orig_ctor orig,
        Player self, Vector2 pos, PlayerSpriteMode spriteMode)
    {
        orig(self, pos, spriteMode);

        DisableTweaks();

        self.Add(new Components.WallCoyoteFramesComponent()); // adding tweak components
        self.Add(new Components.RefillCoyoteComponent());
    }
    public static void DisableTweaksOnLoad(On.Celeste.LevelLoader.orig_ctor orig,
        LevelLoader self, Session session, Vector2? startPos)
    {
        orig(self, session, startPos);

        DisableTweaks();
    }
    private static void DisableTweaks()
    {
        var s = LeniencyHelperModule.Session;

        foreach (string tweak in LeniencyHelperModule.TweakList)
        {
            SetTriggerTweak(tweak, false);
        }

        if (s.BindList is not null)
            s.BindList = Array.Empty<InputRequiresBlockboostTrigger.BindInfo>();

        s.airMovementDisabled = false;
        s.clearBlockBoostActivated = false;
    }

    public static void ResetPlayerSettings()
    {
        List<string> allSettings = new List<string>();
        foreach (List<string> setting in AssociatedTweaks.Values.Where(list => list != null))
            allSettings.AddRange(setting);

        foreach (string setting in allSettings)
        {
            SetPlayerSetting(setting, LeniencyHelperModule.DefaultSettings.Get(setting));
        }
    }
    #endregion


    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//


    #region subsettings
    public static T GetSetting<T>(string name, string tweakName)
    {
        if (LeniencyHelperModule.Settings.PlayerTweaks[tweakName] == true)
        {
            return (T)(GetPlayerSetting(name) != null ? GetPlayerSetting(name) : GetDefaultSetting(name));
        }
        if (LeniencyHelperModule.Settings.PlayerTweaks[tweakName] == false)
        {
            return (T)GetDefaultSetting(name);
        }
        return (T)(LeniencyHelperModule.Session.UseController[tweakName] ?
            (GetControllerSetting(name) != null ? GetControllerSetting(name) : GetDefaultSetting(name))
            : (GetTriggerSetting(name) != null ? GetTriggerSetting(name) : GetDefaultSetting(name)));
    }
    public static T GetDefaultSetting<T>(string name) => (T)LeniencyHelperModule.DefaultSettings.Get(name);
    public static object GetDefaultSetting(string name) => LeniencyHelperModule.DefaultSettings.Get(name);

    public static object GetPlayerSetting(string name) => LeniencyHelperModule.Settings.PlayerSettings.Get(name);
    public static object GetTriggerSetting(string name) => LeniencyHelperModule.Session.TriggerSettings.Get(name);
    public static object GetControllerSetting(string name) => LeniencyHelperModule.Session.ControllerSettings.Get(name);

    public static void SetPlayerSetting(string name, object value) => LeniencyHelperModule.Settings.PlayerSettings.Set(name, value);
    public static void SetTriggerSetting(string name, object value) => LeniencyHelperModule.Session.TriggerSettings.Set(name, value);
    public static void SetControllerSetting(string name, object value) => LeniencyHelperModule.Session.ControllerSettings.Set(name, value);

    #endregion

    public static Dictionary<string, object> GetSettingsFromData(EntityData data, string tweakName)
    {
        Dictionary<string, object> result = null;

        foreach (string setting in AssociatedTweaks[tweakName])
        {
            string fromDialog;
            if (setting.ToLower().Contains("inframe"))
                fromDialog = Dialog.Clean("MODOPTIONS_LENIENCYHELPER_SETTINGS_COUNTINFRAMES", Dialog.Languages["english"]);
            else fromDialog = Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_SETTINGS_{setting.ToUpper()}", Dialog.Languages["english"]);

            string settingInData = "";
            for (int c = 0; c < fromDialog.Length; c++) //converting to CamelCase
            {
                if (fromDialog[c] == ' ') continue;

                if (c == 0 || fromDialog[c - 1] == ' ') settingInData += fromDialog[c].ToString().ToUpper();
                else settingInData += fromDialog[c];
            }

            object value = null;

            Type settingType = LeniencyHelperModule.DefaultSettings.Get(setting).GetType();
            if (settingType == typeof(bool)) value = data.Bool(settingInData, GetDefaultSetting<bool>(setting));
            else if (settingType == typeof(int)) value = data.Int(settingInData, GetDefaultSetting<int>(setting));
            else if (settingType == typeof(float)) value = data.Float(settingInData, GetDefaultSetting<float>(setting));
            else if (settingType == typeof(string)) value = data.String(settingInData, GetDefaultSetting<string>(setting));
            else if (settingType == typeof(LeniencyHelperModule.Dirs))
                value = data.Enum(settingInData, GetDefaultSetting<LeniencyHelperModule.Dirs>(setting));

            if (result == null) result = new Dictionary<string, object>();
            result.Add(setting, value);
        }

        return result;
    }

    public static Dictionary<string, List<string>> AssociatedTweaks = new Dictionary<string, List<string>>
    {
        { "AutoSlowfall", null },
        { "BackwardsRetention", null },
        { "BufferableClimbtrigger", new List<string>{ "onNormalUpdate", "onDash" } },
        { "BufferableExtends", new List<string>{ "forceWaitForRefill" } },
        { "ConsistentDashOnDBlockExit", null },
        { "CornerWaveLeniency", null },
        { "CustomBufferTime", new List<string>{ "countBufferTimeInFrames", "JumpBufferTime", "DashBufferTime", "DemoBufferTime" } },
        { "CustomDashbounceTiming", new List<string>{ "dashbounceTiming", "countDashbounceTimingInFrames" } },
        { "DashCDIgnoreFFrames", null },
        { "DirectionalReleaseProtection", new List<string>{ "DirectionalBufferTime", "CountProtectionTimeInFrames", "dashDir", "jumpDir" } },
        { "DisableBackboost", null },
        { "DisableForceMove", null },
        { "DynamicCornerCorrection", new List<string>{ "FloorCorrectionTiming", "WallCorrectionTiming", "ccorectionTimingInFrames" } },
        { "DynamicWallLeniency", new List<string>{ "wallLeniencyTiming", "countWallTimingInFrames" } },
        { "ExtendBufferOnFreezeAndPickup", new List<string>{ "ExtendBufferOnPickup", "ExtendBufferOnFreeze" } },
        { "ForceCrouchDemodash", null },
        { "IceWallIncreaseWallLeniency", new List<string>{ "iceWJLeniency" } },
        { "InstantAcceleratedJumps", null },
        { "InstantClimbHop", null },
        { "NoFailedTech", null },
        { "RefillDashInCoyote", new List<string>{ "RefillCoyoteTime", "CountRefillCoyoteTimeInFrames" } },
        { "RemoveDBlockCCorection", null },
        { "RetainSpeedCornerboost", new List<string>{ "RetainCbSpeedTime", "countRetainTimeInFrames" } },
        { "SolidBlockboostProtection", new List<string>{ "bboostSaveTime", "countSolidBoostSaveTimeInFrames" } },
        { "SuperOverWalljump", null },
        { "WallAttraction", new List<string>{ "wallApproachTime", "countAttractionTimeInFrames" } },
        { "WallCoyoteFrames", new List<string>{ "wallCoyoteTime", "countWallCoyoteTimeInFrames" } },
    };

    public static Dictionary<string, FieldInfo> SettingListFields = new Dictionary<string, FieldInfo>();
}