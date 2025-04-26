using System;
using System.Collections.Generic;
using System.Reflection;
using YamlDotNet.Serialization;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper;

public class SettingList
{
    // BufferableClimbtrigger
    public bool onNormalUpdate = true;
    public bool onDash = true;

    // BufferableExtends
    public bool forceWaitForRefill = false;

    // ConsistentDashOnDBlockExit
    public bool resetDashCDonLeave = true;

    // CustomBufferTime
    public float JumpBufferTime = 0.08f;
    public float DashBufferTime = 0.08f;
    public float DemoBufferTime = 0.08f;
    public bool countBufferTimeInFrames = false;

    // CustomDashbounceTiming
    public float dashbounceTiming = 0.05f;
    public bool countDashbounceTimingInFrames = false;

    // DirectionalReleaseProtection
    public Dirs dashDir = Dirs.Down;
    public Dirs jumpDir = Dirs.None;
    public float DirectionalBufferTime = 0.1f;
    public bool CountProtectionTimeInFrames = false;

    // DynamicCornerCorrection
    public float FloorCorrectionTiming = 0.05f;
    public float WallCorrectionTiming = 0.05f;
    public bool ccorectionTimingInFrames = false;

    // DynamicWallLeniency
    public float wallLeniencyTiming = 0.05f;
    public bool countWallTimingInFrames = false;

    // ExtendBufferOnFreezeAndPickup
    public bool ExtendBufferOnFreeze = true;
    public bool ExtendBufferOnPickup = false;

    // IceWallIncreaseWallLeniency
    public int iceWJLeniency = 3;

    // RefillDashInCoyote
    public float RefillCoyoteTime = 0.05f;
    public bool CountRefillCoyoteTimeInFrames = false;

    // RetainSpeedCornerboost
    public float RetainCbSpeedTime = 0.1f;
    public bool countRetainTimeInFrames = false;

    // SolidBlockboostProtection
    public float bboostSaveTime = 0.1f;
    public bool countSolidBoostSaveTimeInFrames = false;

    // WallAttraction
    public float wallApproachTime = 0.08f;
    public bool countAttractionTimeInFrames = false;

    // WallCoyoteFrames
    public float wallCoyoteTime = 0.08f;
    public bool countWallCoyoteTimeInFrames = false;
    
    public object Get(string setting)
    {
        return SettingMaster.SettingListFields[setting].GetValue(this);
    }
    public void Set(string setting, object value)
    {
        SettingMaster.SettingListFields[setting].SetValue(this, value);
    }
}
