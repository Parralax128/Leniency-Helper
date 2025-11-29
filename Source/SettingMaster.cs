using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Reflection;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper;
public static class SettingMaster
{
    #region disable tweaks on restart

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

    public static void DisableTweaksOnRespawn(On.Celeste.Player.orig_ctor orig,
        Player self, Vector2 pos, PlayerSpriteMode spriteMode)
    {
        orig(self, pos, spriteMode);

        DisableTweaks();

        self.Add(new Components.WallCoyoteFramesComponent());
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

        foreach (Tweak tweak in LeniencyHelperModule.TweakList)
        {
            SetTriggerTweak(tweak, false);
        }

        if (s.BindList != null) s.BindList.Clear();

        s.airMovementDisabled = false;
        s.clearBlockBoostActivated = false;
    }
    #endregion

    #region tweaks

    public static bool GetTweakEnabled(Tweak tweak, bool ignoreOverride = false)
    {
        var session = LeniencyHelperModule.Session;

        if (!ignoreOverride && session.OverridePlayerSettings && session.OverrideTweaks[tweak] != null)
            return session.OverrideTweaks[tweak].Value;

        if (LeniencyHelperModule.Settings.PlayerTweaks[tweak] != null)
            return LeniencyHelperModule.Settings.PlayerTweaks[tweak].Value;

        if (!ignoreOverride && session.OverrideTweaks[tweak] != null)
            return session.OverrideTweaks[tweak].Value;


        return session.UseController[tweak] ?
            session.ControllerTweaks[tweak] : session.TriggerTweaks[tweak];
    }
    public static void SetPlayerTweak(Tweak tweak, bool? newValue) => LeniencyHelperModule.Settings.PlayerTweaks[tweak] = newValue;
    public static void SetTriggerTweak(Tweak tweak, bool newValue) => LeniencyHelperModule.Session.TriggerTweaks[tweak] = newValue;
    public static void SetControllerTweak(Tweak tweak, bool newValue) => LeniencyHelperModule.Session.ControllerTweaks[tweak] = newValue;
    public static void SetUseController(Tweak tweak, bool useController) => LeniencyHelperModule.Session.UseController[tweak] = useController;

    public static void ResetPlayerSettings()
    {
        List<string> allSettings = new List<string>();
        foreach (List<string> setting in AssociatedSettings.Values.Where(list => list != null))
            allSettings.AddRange(setting);

        foreach (string setting in allSettings)
        {
            SetPlayerSetting(setting, LeniencyHelperModule.DefaultSettings[setting]);
        }
    }
    #endregion

    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

    #region subsettings

    public static float GetTime(string setting, Tweak tweakName)
    {
        string modeName = $"count{char.ToUpper(setting[0])}{setting.Substring(1)}InFrames";
        return GetSetting<float>(setting, tweakName) * (GetSetting<bool>(modeName, tweakName) ? 
            Monocle.Engine.DeltaTime : 1f);
    }
    public static T GetSetting<T>(string name, Tweak tweakName)
    {
        if (LeniencyHelperModule.Settings.PlayerTweaks[tweakName] == true)
        {
            return (T)(GetPlayerSetting(name) ?? GetDefaultSetting(name));
        }
        if (LeniencyHelperModule.Settings.PlayerTweaks[tweakName] == false)
        {
            return (T)GetDefaultSetting(name);
        }
        return (T)(LeniencyHelperModule.Session.UseController[tweakName] ?
            (GetControllerSetting(name) ?? GetDefaultSetting(name))
            : (GetTriggerSetting(name) ?? GetDefaultSetting(name)));
    }
    public static T GetDefaultSetting<T>(string name) => (T)LeniencyHelperModule.DefaultSettings[name];
    public static object GetDefaultSetting(string name) => LeniencyHelperModule.DefaultSettings[name];

    public static object GetPlayerSetting(string name) => LeniencyHelperModule.Settings.PlayerSettings[name];
    public static object GetTriggerSetting(string name) => LeniencyHelperModule.Session.TriggerSettings[name];
    public static object GetControllerSetting(string name) => LeniencyHelperModule.Session.ControllerSettings[name];

    public static void SetPlayerSetting(string name, object value) => LeniencyHelperModule.Settings.PlayerSettings[name] = value;
    public static void SetTriggerSetting(string name, object value) => LeniencyHelperModule.Session.TriggerSettings[name] = value;
    public static void SetControllerSetting(string name, object value) => LeniencyHelperModule.Session.ControllerSettings[name] = value;

    #endregion
    
    public static Dictionary<string, object> GetSettingsFromData(EntityData data, Tweak tweakName)
    {
        Dictionary<string, object> result = null;

        foreach (string setting in AssociatedSettings[tweakName])
        {
            string fromDialog;
            if (setting.ToLower().Contains("inframe"))
                fromDialog = Dialog.Clean("MODOPTIONS_LENIENCYHELPER_SETTINGS_COUNTINFRAMES", Dialog.Languages["english"]);
            else fromDialog = Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_SETTINGS_{setting.ToUpper()}", Dialog.Languages["english"]);

            string settingInData = "";
            for (int c = 0; c < fromDialog.Length; c++) //converting to CamelCase
            {
                if (fromDialog[c] == ' ' || fromDialog[c] == '-') continue;

                if (c == 0 || fromDialog[c - 1] == ' ' || fromDialog[c - 1] == '-') settingInData += char.ToUpper(fromDialog[c]);
                else settingInData += fromDialog[c];
            }

            object value = null;

            Type settingType = LeniencyHelperModule.DefaultSettings[setting].GetType();
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

    public static Dictionary<Tweak, List<string>> AssociatedSettings = new Dictionary<Tweak, List<string>>
    {
        { Tweak.AutoSlowfall, new List<string> {"techOnly", "delayedJumpRelease", "releaseDelay", "countReleaseDelayInFrames"} },
        { Tweak.BackboostProtection, new List<string>{ "earlyBackboostTiming", "lateBackboostTiming", "countBackboostTimingInFrames" } },
        { Tweak.BackwardsRetention, null },
        { Tweak.BufferableClimbtrigger, new List<string>{ "onNormalUpdate", "onDash" } },
        { Tweak.BufferableExtends, new List<string>{ "forceWaitForRefill", "extendsTiming", "countExtendsTimingInFrames" } },
        { Tweak.ConsistentDashOnDBlockExit, new List<string>{ "resetDashCDonLeave" } },
        { Tweak.ConsistentWallboosters, new List<string>{ "instantWallboosterAcceleration", "newWallboosterAcceleration",
            "consistentWallboosterBlockboost", "bufferableWallboosterMaxjumps" } },
        { Tweak.CornerWaveLeniency, null },
        { Tweak.CrouchOnBonk, null },
        { Tweak.CustomBufferTime, new List<string>{ "countBufferTimeInFrames", "JumpBufferTime", "DashBufferTime", "DemoBufferTime" } },
        { Tweak.CustomDashbounceTiming, new List<string>{ "dashbounceTiming", "countDashbounceTimingInFrames" } },
        { Tweak.CustomSnapDownDistance, new List<string>{ "staticSnapdownDistance", "snapdownTiming", "dynamicSnapdownDistance", "countSnapdownTimingInFrames" } },
        { Tweak.DashCDIgnoreFFrames, null },
        { Tweak.DelayedClimbtrigger, new List<string>{ "triggerDelay", "countTriggerDelayInFrames" } },
        { Tweak.DirectionalReleaseProtection, new List<string>{ "DirectionalBufferTime", "CountProtectionTimeInFrames", "dashDir", "jumpDir",
        "affectFeathers", "affectSuperdashes" } },
        { Tweak.DisableBackboost, null },
        { Tweak.DisableForcemovedTech, null },
        { Tweak.DynamicCornerCorrection, new List<string>{ "FloorCorrectionTiming", "WallCorrectionTiming", "ccorectionTimingInFrames" } },
        { Tweak.DynamicWallLeniency, new List<string>{ "wallLeniencyTiming", "countWallLeniencyTimingInFrames" } },
        { Tweak.ExtendBufferOnFreezeAndPickup, new List<string>{ "ExtendBufferOnPickup", "ExtendBufferOnFreeze" } },
        { Tweak.ExtendDashAttackOnPickup, new List<string>{ "attackExtendTime", "countAttackExtendTimeInFrames" } },
        { Tweak.ForceCrouchDemodash, null },
        { Tweak.GultraCancel, new List<string> { "cancelTime", "countCancelTimeInFrames" } },
        { Tweak.IceWallIncreaseWallLeniency, new List<string>{ "iceWJLeniency" } },
        { Tweak.InstantAcceleratedJumps, null },
        { Tweak.NoFailedTech, new List<string>{ "protectedTechTime", "countProtectedTechTimeInFrames" } },        
        { Tweak.ManualDreamhyperLeniency, null },
        { Tweak.RefillDashInCoyote, new List<string>{ "RefillCoyoteTime", "CountRefillCoyoteTimeInFrames" } },
        { Tweak.RemoveDBlockCCorection, null },
        { Tweak.RespectInputOrder, new List<string> { "affectGrab" } },
        { Tweak.RetainSpeedCornerboost, new List<string>{ "RetainCbSpeedTime", "countRetainTimeInFrames" } },
        { Tweak.LateReverses, new List<string>{ "redirectTime", "countRedirectTimeInFrames" } },
        { Tweak.SolidBlockboostProtection, new List<string>{ "bboostSaveTime", "countSolidBoostSaveTimeInFrames" } },
        { Tweak.SuperdashSteeringProtection, null },
        { Tweak.SuperOverWalljump, null },
        { Tweak.WallAttraction, new List<string>{ "wallApproachTime", "countWallApproachTimeInFrames", "staticApproachDistance", "useDynamicApproachDistance" } },
        { Tweak.WallCoyoteFrames, new List<string>{ "wallCoyoteTime", "countWallCoyoteTimeInFrames" } },
    };

    public static Dictionary<string, FieldInfo> SettingListFields { get; set; } = new Dictionary<string, FieldInfo>();
}