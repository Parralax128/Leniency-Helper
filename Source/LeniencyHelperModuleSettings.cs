using static Celeste.Mod.LeniencyHelper.Tweaks.DirectionalReleaseProtection;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System.Linq;
using System.ComponentModel.Design;
using System.Collections.Immutable;

namespace Celeste.Mod.LeniencyHelper;

public class LeniencyHelperModuleSettings : EverestModuleSettings 
{
    public Dictionary<string, bool?> TweaksByPlayer
    {
        get;
        set; 
    } = GetDefaultSettings();
    public static Dictionary<string, bool?> GetDefaultSettings()
    {
        Dictionary<string, bool?> dict = new Dictionary<string, bool?>();
        foreach(string tweak in LeniencyHelperModule.Tweaks)
        {
            dict.Add(tweak, null);
        }
        return dict;
    }

    public object GetValue(string name)
    {
        try { return this.GetType().GetProperty(name).GetValue(this); }
        catch { Log($"Settings failed at getting {name}"); return null; }
    }

    public void SetValue(string name, object value)
    {
        try { this.GetType().GetProperty(name).SetValue(this, value); }
        catch { Log($"Settings failed at setting {name}"); }
    }
    public object GetSetting(string tweakName, string settingName)
    {
        if (LeniencyHelperModule.Settings.TweaksByPlayer[tweakName] != null)
        {
            return LeniencyHelperModule.Settings.GetValue(settingName);
        }
        else
        {
            string startWithUpper = settingName[0].ToString().ToUpper() +
                settingName.Substring(1);

            if (LeniencyHelperModule.Session is null) return null;
            return LeniencyHelperModule.Session.GetValue("map" + startWithUpper);
        }
    }
    public object GetSettingByIndexPriority(int index, string settingName)
    {
        if (index == 0)
        {
            string startWithUpper = settingName[0].ToString().ToUpper() +
                settingName.Substring(1);

            return LeniencyHelperModule.Session.GetValue("map" + startWithUpper);
        }
        else return LeniencyHelperModule.Settings.GetValue(settingName);
    }

    #region in-game settings

    //BufferableClimbtrigger
    public bool onlyOnClimbjumps { get; set; } = false;

    //BufferableExtends
    public bool forceWaitForRefill { get; set; } = false;

    //ConsistentDashOnDBlockExit
    public bool resetDashCDonLeave { get; set; } = true;

    //CornerWaveLeniency
    public bool allowSpikedFloor { get; set; } = false;

    //CustomBufferTime
    public Dictionary<Inputs, float> buffers { get; set; } = new Dictionary<Inputs, float>
    {
        { Inputs.Jump, 0.08f },
        { Inputs.Dash, 0.08f },
        { Inputs.Demo, 0.08f }
    };
    public bool countBufferTimeInFrames { get; set; } = false;

    //DirectionalReleaseProtection
    public Dirs dashDir { get; set; } = Dirs.Down;
    public Dirs jumpDir { get; set; } = Dirs.All;
    public float DirectionalBufferTime { get; set; } = 0.1f;
    public bool CountProtectionTimeInFrames { get; set; } = false;

    //DynamicCornerCorrection
    public float FloorCorrectionTiming { get; set; } = 0.1f;
    public float WallCorrectionTiming { get; set; } = 0.05f;
    public bool ccorectionTimingInFrames { get; set; } = false;

    //DynamicWallLeniency
    public bool countWallTimingInFrames { get; set; } = false;
    public float wallLeniencyTiming { get; set; } = 0.05f;

    //FFramesExtendBuffer
    public bool ExtendBufferOnFreeze { get; set; } = true;
    public bool ExtendBufferOnPickup { get; set; } = true;

    //FrozenReverses
    public float reversedFreezeTime { get; set; } = 2f;
    public bool countReversedInFrames { get; set; } = true;

    //IceWallIncreaseWBleniency
    public int iceWJLeniency { get; set; } = 3;

    //RefillDashInCoyote
    public float RefillCoyoteTime { get; set; } = 0.05f;
    public bool CountRefillCoyoteTimeInFrames { get; set; } = false;

    //RetainSpeedCornerboost
    public float RetainCbSpeedTime { get; set; } = 0.25f;
    public bool countRetainTimeInFrames { get; set; } = false;

    //SuperOverWalljumpPriority
    public float bboostSaveTime { get; set; } = 0.1f;
    public bool countSolidBoostSaveTimeInFrames { get; set; } = false;

    //WallAttraction
    public float wallApproachTime { get; set; } = 0.05f;
    public bool countAttractionTimeInFrames { get; set; } = false;

    //WallCoyoteFrames
    public float wallCoyoteTime { get; set; } = 0.1f;
    public bool countWallCoyoteTimeInFrames { get; set; } = false;

    #endregion
}