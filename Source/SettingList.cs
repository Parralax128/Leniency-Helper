using VivHelper.Entities;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper;

public class SettingList
{
    // BackboostProtection
    public float earlyBackboostTiming = 0.35f;
    public float lateBackboostTiming = 0.1f;
    public bool countBackboostTimingInFrames = false;

    // BufferableClimbtrigger
    public bool onNormalUpdate = true;
    public bool onDash = true;

    // BufferableExtends
    public bool forceWaitForRefill = false;
    public float extendsTiming = 0.08f;

    // ConsistentDashOnDBlockExit
    public bool resetDashCDonLeave = true;

    // ConsistentWallboosters
    public bool consistentWallboosterBlockboost = true;
    public bool instantWallboosterAcceleration = false;
    public int newWallboosterAcceleration = 10;
    public bool bufferableWallboosterMaxjumps = true;

    // CustomBufferTime
    public float JumpBufferTime = 0.08f;
    public float DashBufferTime = 0.08f;
    public float DemoBufferTime = 0.08f;
    public bool countBufferTimeInFrames = false;

    // CustomDashbounceTiming
    public float dashbounceTiming = 0.05f;
    public bool countDashbounceTimingInFrames = false;

    // CustomSnapDownDistance
    public int staticSnapdownDistance = 3;
    public float snapdownTiming = 0.05f;
    public bool dynamicSnapdownDistance = true;
    public bool countSnapdownTimingInFrames = false;

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

    // NoFailedTech
    public float protectedTechTime = 0.1f;
    public bool countProtectedTechTimeInFrames = false;
    
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
    public bool consistentBlockboost = true;
    
    public object Get(string setting)
    {
        return SettingMaster.SettingListFields[setting].GetValue(this);
    }
    public void Set(string setting, object value)
    {
        SettingMaster.SettingListFields[setting].SetValue(this, value);
    }
}
