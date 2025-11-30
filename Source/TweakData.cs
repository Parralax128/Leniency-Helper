using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.LeniencyHelper;
public static class TweakData
{
    private static CompoundSetting<FlexDistance> FlexDistanceSetting(string name) =>
        new CompoundSetting<FlexDistance>(name, new SettingContainer {
                new Setting<int>("StaticDistance", 4, 0, 32),
                new Setting<bool>("Dynamic", false),
                new Setting<Time>("Time", new Time(0f), new Time(0f), new Time(0.25f))
        }, (value, subsettings, source) => {
            value.StaticValue = subsettings.Get<int>("StaticDistance", source);
            value.Dynamic = subsettings.Get<bool>("Dynamic", source);
            value.Timer = subsettings.Get<Time>("Time", source);
        });

    #region Tweak initialization

    public class TweakList : IEnumerable<TweakState>
    {
        private List<TweakState> tweakStates;

        public IEnumerator<TweakState> GetEnumerator() => tweakStates.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => tweakStates.GetEnumerator();

        public TweakList()
        {
            int size = System.Enum.GetValues<Tweak>().Length;
            tweakStates = new List<TweakState>(size);
            tweakStates.AddRange(Enumerable.Repeat<TweakState>(null, size));
        }
        public void Add(TweakState tweakState) => tweakStates[(int)tweakState.Tweak] = tweakState;

        public TweakState this[Tweak tweak]
        {
            get
            {
                TweakState result = tweakStates[(int)tweak];
                if (tweak == result.Tweak) return result;
                throw new AmbiguousMatchException($"{tweak} = {(int)tweak} points on {result.Tweak} = {(int)result.Tweak}!");
            }
            set => tweakStates[(int)tweak] = value;
        }
    }


    public static TweakList Tweaks = new()
    {
        new TweakState(Tweak.AutoSlowfall, new SettingContainer {
            new Setting<bool>("TechOnly", true),
            new Setting<bool>("DelayedJumpRelease", true),
            new Setting<Time>("ReleaseDelay", new Time(0.2f), new Time(0f), new Time(1f))
        }),

        new TweakState(Tweak.BackboostProtection, new SettingContainer {
            new Setting<Time>("EarlyBackboostTiming", new Time(0.35f), new Time(0f), new Time(1f)),
            new Setting<Time>("LateBackboostTiming", new Time(0.1f), new Time(0f), new Time(1f))
        }),

        new TweakState(Tweak.BackwardsRetention),

        new TweakState(Tweak.BufferableClimbtrigger, new SettingContainer {
            new Setting<bool>("OnNormalUpdate", true),
            new Setting<bool>("OnDash", true)
        }),

        new TweakState(Tweak.BufferableExtends, new SettingContainer {
            new Setting<bool>("ForceWaitForRefill", false),
            new Setting<bool>("ExtendTiming", true)
        }),

        new TweakState(Tweak.ConsistentDashOnDBlockExit, new SettingContainer {
            new Setting<bool>("ResetDashCDonLeave", true)
        }),

        new TweakState(Tweak.ConsistentWallboosters, new SettingContainer {
            new Setting<bool>("ConsistentBlockboost", true),
            new Setting<bool>("InstantAcceleration", false),
            new Setting<int>("CustomAcceleration", 10),
            new Setting<bool>("BufferableMaxjumps", true)
        }),

        new TweakState(Tweak.CornerWaveLeniency),

        new TweakState(Tweak.CrouchOnBonk),

        new TweakState(Tweak.CustomBufferTime, new SettingContainer {
            new Setting<Time>("JumpBufferTime", new Time(0.08f), new Time(0f), new Time(0.25f)),
            new Setting<Time>("DashBufferTime", new Time(0.08f), new Time(0f), new Time(0.25f)),
            new Setting<Time>("DemoDashBufferTime", new Time(0.08f), new Time(0f), new Time(0.25f))
        }),

        new TweakState(Tweak.CustomDashbounceTiming, new SettingContainer {
            new Setting<Time>("Timing", new Time(0.1f))
        }),

        new TweakState(Tweak.CustomSnapDownDistance, new SettingContainer {
            FlexDistanceSetting("SnapDownDistance")
        }),

        new TweakState(Tweak.DashCDIgnoreFFrames),

        new TweakState(Tweak.DelayedClimbtrigger, new SettingContainer {
            new Setting<Time>("MaxDelay", new Time(0.25f), new Time(0f), new Time(0.5f))
        }),

        new TweakState(Tweak.DirectionalReleaseProtection, new SettingContainer {
            new Setting<LeniencyHelperModule.Dirs>("DashDir", LeniencyHelperModule.Dirs.Down),
            new Setting<LeniencyHelperModule.Dirs>("JumpDir", LeniencyHelperModule.Dirs.None),
            new Setting<Time>("ProtectionTime", new Time(0.1f), new Time(0f), new Time(0.25f)),
            new Setting<bool>("AffectFeathers", false),
            new Setting<bool>("AffectSuperdashes", false)
        }),

        new TweakState(Tweak.DisableBackboost),

        new TweakState(Tweak.DisableForcemovedTech),

        new TweakState(Tweak.DynamicCornerCorrection, new SettingContainer {
            new Setting<Time>("FloorCorrectionTiming", new Time(0.05f), new Time(0f), new Time(0.25f)),
            new Setting<Time>("WallCorrectionTiming", new Time(0.05f), new Time(0f), new Time(0.25f))
        }),

        new TweakState(Tweak.DynamicWallLeniency, new SettingContainer {
            new Setting<Time>("Timing", new Time(0.05f), new Time(0f), new Time(0.25f))
        }),

        new TweakState(Tweak.ExtendBufferOnFreezeAndPickup, new SettingContainer {
            new Setting<bool>("OnFreeze", true),
            new Setting<bool>("OnPickup", false)
        }),

        new TweakState(Tweak.ExtendDashAttackOnPickup, new SettingContainer {
            new Setting<Time>("ExtendTime", new Time(0.1f), new Time(0f), new Time(0.5f))
        }),

        new TweakState(Tweak.ForceCrouchDemodash),

        new TweakState(Tweak.GultraCancel, new SettingContainer {
            new Setting<Time>("MaxCancelDelay", new Time(0.08f), new Time(0f))
        }),

        new TweakState(Tweak.IceWallIncreaseWallLeniency, new SettingContainer {
            new Setting<int>("ExtraWalljumpDistance", 4, 0, 12)
        }),

        new TweakState(Tweak.InstantAcceleratedJumps),

        new TweakState(Tweak.LateReverses, new SettingContainer {
            new Setting<Time>("ReverseTiming", new Time(0.08f), new Time(0f), new Time(0.25f))
        }),

        new TweakState(Tweak.ManualDreamhyperLeniency),

        new TweakState(Tweak.NoFailedTech, new SettingContainer {
            new Setting<Time>("ProtectedTechTime", new Time(0.1f), new Time(0f))
        }),

        new TweakState(Tweak.RefillDashInCoyote, new SettingContainer {
            new Setting<Time>("RefillCoyoteTime", new Time(0.05f), new Time(0f), new Time(0.2f))
        }),

        new TweakState(Tweak.RemoveDBlockCCorection),

        new TweakState(Tweak.RespectInputOrder, new SettingContainer {
            new Setting<bool>("AffectGrab", false)
        }),

        new TweakState(Tweak.RetainSpeedCornerboost, new SettingContainer {
            new Setting<Time>("MaxRetainTime", new Time(0.1f), new Time(0f))
        }),

        new TweakState(Tweak.SolidBlockboostProtection, new SettingContainer {
            new Setting<Time>("MaxSaveTime", new Time(0.1f), new Time(0f), new Time(0.5f))
        }),

        new TweakState(Tweak.SuperdashSteeringProtection),

        new TweakState(Tweak.SuperOverWalljump),

        new TweakState(Tweak.WallAttraction, new SettingContainer {
            FlexDistanceSetting("AttractionDistance")
        }),

        new TweakState(Tweak.WallCoyoteFrames, new SettingContainer {
            new Setting<Time>("WallCoyoteTime", new Time(0.05f), new Time(0f), new Time(0.1f))
        })
    };
    #endregion
}