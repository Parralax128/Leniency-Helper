using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Reflection;
using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.TweakSettings;
using MonoMod.Utils;
using Microsoft.Build.Utilities;

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
            SetTriggerTweak(tweak, null);
        }

        if (s.BindList != null) s.BindList.Clear();

        s.airMovementDisabled = false;
        s.clearBlockBoostActivated = false;
    }
    #endregion

    #region tweaks

    public static void SetTriggerTweak(Tweak tweak, bool? newValue)
    {

        TweakData.Tweaks[tweak].Set(newValue, SettingSource.Trigger);

    }
    public static void SetControllerTweak(Tweak tweak, bool newValue) => TweakData.Tweaks[tweak].Set(newValue, SettingSource.Controller);

    public static void ResetPlayerSettings()
    {
        foreach(Tweak tweak in LeniencyHelperModule.TweakList.Where(t => TweakData.Tweaks[t].Settings != null))
        {
            foreach (AbstractSetting setting in TweakData.Tweaks[tweak].Settings)
            {
                try
                {
                    setting.Reset(SettingSource.Player);
                }
                catch (Exception e)
                {
                    Debug.Warn($"{tweak}/{setting.Name} failed resetting!");
                    Debug.Warn(e);
                }
                
            }
        }
        
    }
    #endregion

    public static Dictionary<string, object> ParseSettingsFromData(EntityData data, Tweak tweak)
    {
        if (!tweak.HasSettings()) return null;

        Dictionary<string, object> result = new();

        foreach (AbstractSetting setting in TweakData.Tweaks[tweak].Settings)
        {
            object parsed = setting.ParseFromData(data);
            if (parsed is Dictionary<string, object> dict)
                result.AddRange(dict);
            else result.Add(setting.Name, parsed);
        }

        return result;
    }
}