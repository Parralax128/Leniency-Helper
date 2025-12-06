using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.LeniencyHelper;

public enum Tweak
{
    AutoSlowfall = 0,
    BackboostProtection = 1,
    BackwardsRetention = 2,
    BufferableClimbtrigger = 3,
    BufferableExtends = 4,
    ConsistentDashOnDBlockExit = 5,
    ConsistentWallboosters = 6,
    CornerWaveLeniency = 7,
    CrouchOnBonk = 8,
    CustomBufferTime = 9,
    CustomDashbounceTiming = 10,
    CustomSnapDownDistance = 11,
    DashCDIgnoreFFrames = 12,
    DelayedClimbtrigger = 13,
    DirectionalReleaseProtection = 14,
    DisableBackboost = 15,
    DisableForcemovedTech = 16,
    DynamicCornerCorrection = 17,
    DynamicWallLeniency = 18,
    ExtendBufferOnFreezeAndPickup = 19,
    ExtendDashAttackOnPickup = 20,
    ForceCrouchDemodash = 21,
    GultraCancel = 22,
    IceWallIncreaseWallLeniency = 23,
    InstantAcceleratedJumps = 24,
    LateReverses = 25,
    ManualDreamhyperLeniency = 26,
    NoFailedTech = 27,
    RefillDashInCoyote = 28,
    RemoveDBlockCCorrection = 29,
    RespectInputOrder = 30,
    RetainSpeedCornerboost = 31,
    SolidBlockboostProtection = 32,
    SuperdashSteeringProtection = 33,
    SuperOverWalljump = 34,
    WallAttraction = 35,
    WallCoyoteFrames = 36
}

public static class TweakExtension
{
    public static bool HasSettings(this Tweak tweak) => TweakData.Tweaks[tweak].Settings != null;
    public static bool Enabled(this Tweak tweak) => TweakData.Tweaks[tweak].Enabled;

    public static SettingSource GetMapSource(this Tweak tweak)
    {
        TweakState state = TweakData.Tweaks[tweak];
        if (state.Get(SettingSource.Trigger) != null) return SettingSource.Trigger;
        return SettingSource.Controller;
    }

    public static bool? SetSetting(this Tweak tweak, string setting, object value, SettingSource source)
        => TweakData.Tweaks[tweak].Settings.Set(setting, source, value);
    public static T GetSetting<T>(this Tweak tweak, string setting)
        => TweakData.Tweaks[tweak].GetSetting<T>(setting);
    public static bool? Get(this Tweak tweak, SettingSource source) => TweakData.Tweaks[tweak].Get(source);

    public static T Advance<T>(this T current, int direction) where T : struct, Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        return values[Array.IndexOf(values, current) + direction];
    }
    public static bool CheckRange<T>(this T current, int direction, out bool min, out bool max) where T : struct, Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));

        int nextIndex = Array.IndexOf(values, current) + direction;

        min = nextIndex >= 0;
        max = nextIndex < values.Length;

        return min && max;
    }
    public static bool CheckRange<T>(this T current, int direction) where T : struct, Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));

        int nextIndex = Array.IndexOf(values, current) + direction;
        return nextIndex >= 0 && nextIndex < values.Length;
    }
}