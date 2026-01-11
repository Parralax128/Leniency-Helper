using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;

namespace Celeste.Mod.LeniencyHelper;

public enum Tweak : int
{
    AutoSlowfall,
    BackboostProtection,
    BackwardsRetention,
    BufferableClimbtrigger,
    BufferableExtends,
    ConsistentDashOnDBlockExit,
    ConsistentWallboosters,
    CornerWaveLeniency,
    CrouchOnBonk,
    CustomBufferTime,
    CustomDashbounceTiming,
    CustomSnapDownDistance,
    DashCDIgnoreFFrames,
    DelayedClimbtrigger,
    DirectionalReleaseProtection,
    DisableBackboost,
    DisableForcemovedTech,
    DynamicCornerCorrection,
    DynamicWallLeniency,
    ExtendBufferOnFreezeAndPickup,
    ExtendDashAttackOnPickup,
    ForceCrouchDemodash,
    GultraCancel,
    IceWallIncreaseWallLeniency,
    InstantAcceleratedJumps,
    LateReverses,
    ManualDreamhyperLeniency,
    NoFailedTech,
    RefillDashInCoyote,
    RemoveDBlockCCorrection,
    RespectInputOrder,
    RetainSpeedCornerboost,
    SolidBlockboostProtection,
    SuperdashSteeringProtection,
    SuperOverWalljump,
    WallAttraction,
    WallCoyoteFrames
}

public static class TweakExtension
{
    public static bool HasSettings(this Tweak tweak) => TweakData.Tweaks[tweak].Settings != null;
    public static bool Enabled(this Tweak tweak) => TweakData.Tweaks[tweak].Enabled;
    public static bool? Get(this Tweak tweak, SettingSource source) => TweakData.Tweaks[tweak].Get(source);

    public static Tweak? FromString(string tweakName)
    {
        if (Enum.TryParse(tweakName, false, out Tweak result)) return result;
        return null;
    }
}