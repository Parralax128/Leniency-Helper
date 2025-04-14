using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using Celeste.Mod.LeniencyHelper.Triggers;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper;
public struct TweakSubSetting
{
    public string tweakName;
    public Type type;

    public object playerValue;

    public object controllerValue;
    public object triggerValue;

    public object vanillaValue;
    public object defaultValue;

    public TweakSubSetting(Type t, string tweak, object vanillaValue, object defaultValue)
    {
        tweakName = tweak;
        this.vanillaValue = vanillaValue;
        this.defaultValue = playerValue = defaultValue;
        triggerValue = controllerValue = null;
        type = t;
    }
    public TweakSubSetting(Type t, string tweak, object defaultVanillaValue)
    {
        tweakName = tweak;
        vanillaValue = defaultValue = playerValue = defaultVanillaValue;
        triggerValue = controllerValue = null;
        type = t;
    }
}
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

    public struct TweakState
    {
        public bool? playerValue;

        public bool triggerValue;
        public bool controllerValue;
        public bool useController;

        public bool MapEnabled => useController ? controllerValue : triggerValue;
        public bool Enabled => playerValue != null ? playerValue.Value : MapEnabled;
        public TweakState(bool? playerValue)
        {
            this.playerValue = playerValue;
            triggerValue = controllerValue = useController = false;
        }
    }
    public static Dictionary<string, TweakState> Tweaks
    {
        get { return LeniencyHelperModule.Session.Tweaks; }
        set { LeniencyHelperModule.Session.Tweaks = value; }
    }

    public static void SetPlayerTweak(string tweak, bool? newValue) => Tweaks[tweak] = Tweaks[tweak] with { playerValue = newValue };
    public static void SetTriggerTweak(string tweak, bool newValue) => Tweaks[tweak] = Tweaks[tweak] with { triggerValue = newValue };
    public static void SetControllerTweak(string tweak, bool newValue) => Tweaks[tweak] = Tweaks[tweak] with { controllerValue = newValue };
    public static void SetUseController(string tweak, bool useController) => Tweaks[tweak] = Tweaks[tweak] with { useController = useController };

    public static void DisableTweaksOnRespawn(On.Celeste.Player.orig_ctor orig,
        Player self, Vector2 pos, PlayerSpriteMode spriteMode)
    {
        orig(self, pos, spriteMode);

        DisableTweaks();
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

        foreach (string tweak in tweakList)
        {
            SetTriggerTweak(tweak, false);
        }

        if (s is not null)
        {
            if (s.BindList is not null)
                s.BindList = Array.Empty<InputRequiresBlockboostTrigger.BindInfo>();

            s.airMovementDisabled = false;
            s.clearBlockBoostActivated = false;
        }
    }
    public static void ResetPlayerSettings()
    {
        foreach (string setting in TweakSettings.Keys)
        {
            SetPlayerSetting(setting, TweakSettings[setting].defaultValue);
        }
    }
    #endregion


    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
    
    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//


    #region subsettings
    public static Dictionary<string, TweakSubSetting> TweakSettings = new Dictionary<string, TweakSubSetting>
    {
        { "onNormalUpdate", new TweakSubSetting(typeof(bool), "BufferableClimbtrigger", false, true) },
        { "onDash", new TweakSubSetting(typeof(bool), "BufferableClimbtrigger", false, true) },

        { "forceWaitForRefill", new TweakSubSetting(typeof(bool), "BufferableExtends", false) },

        { "resetDashCDonLeave", new TweakSubSetting(typeof(bool), "ConsistentDashOnDBlockExit", false, true) },

        { "allowSpikedFloor", new TweakSubSetting(typeof(bool), "CornerWaveLeniency", false) },

        { "JumpBufferTime", new TweakSubSetting(typeof(float), "CustomBufferTime", 0.08f) },
        { "DashBufferTime", new TweakSubSetting(typeof(float), "CustomBufferTime", 0.08f) },
        { "DemoBufferTime", new TweakSubSetting(typeof(float), "CustomBufferTime", 0.08f) },
        { "countBufferTimeInFrames", new TweakSubSetting(typeof(bool), "CustomBufferTime", false) },

        { "dashbounceTiming", new TweakSubSetting(typeof(float), "CustomDashbounceTiming", 0.05f) },
        { "countDashbounceTimingInFrames", new TweakSubSetting(typeof(bool), "CustomDashbounceTiming", false) },

        { "dashDir", new TweakSubSetting(typeof(Dirs), "DirectionalReleaseProtection", Dirs.None, Dirs.Down) },
        { "jumpDir", new TweakSubSetting(typeof(Dirs), "DirectionalReleaseProtection", Dirs.None) },
        { "DirectionalBufferTime", new TweakSubSetting(typeof(float), "DirectionalReleaseProtection", 0f, 0.1f) },
        { "CountProtectionTimeInFrames", new TweakSubSetting(typeof(bool), "DirectionalReleaseProtection", false) },

        { "FloorCorrectionTiming", new TweakSubSetting(typeof(float), "DynamicCornerCorrection", 0f, 0.05f) },
        { "WallCorrectionTiming", new TweakSubSetting(typeof(float), "DynamicCornerCorrection", 0f, 0.05f) },
        { "ccorectionTimingInFrames", new TweakSubSetting(typeof(bool), "DynamicCornerCorrection", false) },

        { "wallLeniencyTiming", new TweakSubSetting(typeof(float), "DynamicWallLeniency", 0f, 0.05f) },
        { "countWallTimingInFrames", new TweakSubSetting(typeof(bool), "DynamicWallLeniency", false) },

        { "ExtendBufferOnFreeze", new TweakSubSetting(typeof(bool), "ExtendBufferOnFreezeAndPickup", false, true) },
        { "ExtendBufferOnPickup", new TweakSubSetting(typeof(bool), "ExtendBufferOnFreezeAndPickup", false) },

        { "iceWJLeniency", new TweakSubSetting(typeof(int), "IceWallIncreaseWallLeniency", 0, 3) },

        { "RefillCoyoteTime", new TweakSubSetting(typeof(float), "RefillDashInCoyote", 0f, 0.05f) },
        { "CountRefillCoyoteTimeInFrames", new TweakSubSetting(typeof(bool), "RefillDashInCoyote", false) },

        { "RetainCbSpeedTime", new TweakSubSetting(typeof(float), "RetainSpeedCornerboost", 0f, 0.1f) },
        { "countRetainTimeInFrames", new TweakSubSetting(typeof(bool), "RetainSpeedCornerboost", false) },

        { "bboostSaveTime", new TweakSubSetting(typeof(float), "SolidBlockboostProtection", 0f, 0.1f) },
        { "countSolidBoostSaveTimeInFrames", new TweakSubSetting(typeof(bool), "SolidBlockboostProtection", false) },

        { "wallApproachTime", new TweakSubSetting(typeof(float), "WallAttraction", 0f, 0.08f) },
        { "countAttractionTimeInFrames", new TweakSubSetting(typeof(bool), "WallAttraction", false) },

        { "wallCoyoteTime", new TweakSubSetting(typeof(float), "WallCoyoteFrames", 0f, 0.08f) },
        { "countWallCoyoteTimeInFrames", new TweakSubSetting(typeof(bool), "WallCoyoteFrames", false) }
    };
    public static T GetSetting<T>(string name)
    {
        string tweakName = TweakSettings[name].tweakName;
        if (Tweaks[tweakName].playerValue == true)
        {
            return (T)(TweakSettings[name].playerValue != null ? TweakSettings[name].playerValue : TweakSettings[name].defaultValue);
        }
        if (Tweaks[tweakName].playerValue == false)
        {
            return (T)TweakSettings[name].vanillaValue;
        }
        return (T)(Tweaks[tweakName].useController ?
            (TweakSettings[name].controllerValue != null ? TweakSettings[name].controllerValue : TweakSettings[name].vanillaValue)
            : (TweakSettings[name].triggerValue != null ? TweakSettings[name].triggerValue : TweakSettings[name].vanillaValue));
    }
    public static T GetDefaultSetting<T>(string name) => (T)TweakSettings[name].defaultValue;
    public static object GetDefaultSetting(string name) => TweakSettings[name].defaultValue;

    public static void SetPlayerSetting(string name, object value)
    {
        TweakSettings[name] = TweakSettings[name] with { playerValue = value };
        LeniencyHelperModule.Settings.SavedTweakSettings[name] = value;
    }
    public static void SetTriggerSetting(string name, object value) => 
        TweakSettings[name] = TweakSettings[name] with { triggerValue = value };
    public static void SetControllerSetting(string name, object value) => 
        TweakSettings[name] = TweakSettings[name] with { controllerValue = value };
    
    #endregion

    public static Dictionary<string, object> GetSettingsFromData(EntityData data, string tweakName)
    {
        Dictionary<string, object> result = null;

        foreach (KeyValuePair<string, TweakSubSetting> setting in TweakSettings.Where(pair => pair.Value.tweakName == tweakName))
        {
            string settingName = setting.Key;

            string fromDialog;
            if (settingName.ToLower().Contains("inframe"))
                fromDialog = Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_SETTINGS_COUNTINFRAMES", Dialog.Languages["english"]);
            else fromDialog = Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_SETTINGS_{settingName.ToUpper()}", Dialog.Languages["english"]);

            string settingInData = "";
            for (int c = 0; c < fromDialog.Length; c++) //converting to CamelCase
            {
                if (fromDialog[c] == ' ') continue;

                if (c == 0 || fromDialog[c - 1] == ' ') settingInData += fromDialog[c].ToString().ToUpper();
                else settingInData += fromDialog[c];
            }

            object value = null;

            Type settingType = setting.Value.type;
            if (settingType == typeof(bool)) value = data.Bool(settingInData, GetDefaultSetting<bool>(settingName));
            else if (settingType == typeof(int)) value = data.Int(settingInData, GetDefaultSetting<int>(settingName));
            else if (settingType == typeof(float)) value = data.Float(settingInData, GetDefaultSetting<float>(settingName));
            else if (settingType == typeof(string)) value = data.String(settingInData, GetDefaultSetting<string>(settingName));
            else if (settingType == typeof(LeniencyHelperModule.Dirs))
                value = data.Enum(settingInData, GetDefaultSetting<Dirs>(settingName));

            if (result == null) result = new Dictionary<string, object>();
            result.Add(settingName, value);
        }

        return result;
    }
}