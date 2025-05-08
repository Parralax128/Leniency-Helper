using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper;

public class SettingList
{
    // BufferableClimbtrigger
    private bool onNormalUpdate = true;
    private bool onDash = true;

    // BufferableExtends
    private bool forceWaitForRefill = false;
    private float extendsTiming = 0.08f;

    // ConsistentDashOnDBlockExit
    private bool resetDashCDonLeave = true;

    // CustomBufferTime
    private float JumpBufferTime = 0.08f;
    private float DashBufferTime = 0.08f;
    private float DemoBufferTime = 0.08f;
    private bool countBufferTimeInFrames = false;

    // CustomDashbounceTiming
    private float dashbounceTiming = 0.05f;
    private bool countDashbounceTimingInFrames = false;

    // DirectionalReleaseProtection
    private Dirs dashDir = Dirs.Down;
    private Dirs jumpDir = Dirs.None;
    private float DirectionalBufferTime = 0.1f;
    private bool CountProtectionTimeInFrames = false;

    // DynamicCornerCorrection
    private float FloorCorrectionTiming = 0.05f;
    private float WallCorrectionTiming = 0.05f;
    private bool ccorectionTimingInFrames = false;

    // DynamicWallLeniency
    private float wallLeniencyTiming = 0.05f;
    private bool countWallTimingInFrames = false;

    // ExtendBufferOnFreezeAndPickup
    private bool ExtendBufferOnFreeze = true;
    private bool ExtendBufferOnPickup = false;

    // IceWallIncreaseWallLeniency
    private int iceWJLeniency = 3;

    // RefillDashInCoyote
    private float RefillCoyoteTime = 0.05f;
    private bool CountRefillCoyoteTimeInFrames = false;

    // RetainSpeedCornerboost
    private float RetainCbSpeedTime = 0.1f;
    private bool countRetainTimeInFrames = false;

    // SolidBlockboostProtection
    private float bboostSaveTime = 0.1f;
    private bool countSolidBoostSaveTimeInFrames = false;

    // WallAttraction
    private float wallApproachTime = 0.08f;
    private bool countAttractionTimeInFrames = false;

    // WallCoyoteFrames
    private float wallCoyoteTime = 0.08f;
    private bool countWallCoyoteTimeInFrames = false;
    
    public object Get(string setting)
    {
        return SettingMaster.SettingListFields[setting].GetValue(this);
    }
    public void Set(string setting, object value)
    {
        SettingMaster.SettingListFields[setting].SetValue(this, value);
    }
}
