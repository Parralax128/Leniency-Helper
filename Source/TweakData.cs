using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper;
static class TweakData
{
    static SettingContainer FlexDistanceSettings(string name="",
        FlexDistance.Modes defaultMode = FlexDistance.Modes.Static, int defaultStatic = 4, Time defaultTime = null) =>
    new SettingContainer() {
        new Setting<FlexDistance.Modes>(name + "Mode", defaultMode),
        new Setting<int>(name + "StaticDistance", defaultStatic, 0, 12),
        new Setting<Time>(name + "Time", defaultTime ?? 0.05f, 0f, 0.25f)
    };

    public class TweakList : IEnumerable<TweakState>
    {
        List<TweakState> tweakStates;

        public IEnumerator<TweakState> GetEnumerator() => tweakStates.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => tweakStates.GetEnumerator();

        public void ForEach(Action<TweakState> action) => tweakStates.ForEach(action);

        // LINQ mess :P
        public static Dictionary<Tweak, Type> Types = 
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypesSafe()).
            Where(x => x.IsClass && x.BaseType != null && x.BaseType.IsGenericType
            && typeof(Tweaks.AbstractTweak<>) == x.BaseType.GetGenericTypeDefinition()).
            ToDictionary(t => Enum.Parse<Tweak>(t.Name));
            
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

    public static TweakList Tweaks;

    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void SetupTweaklist() { Tweaks = new()
    {
        new TweakState(Tweak.AutoSlowfall, new SettingContainer {
            new Setting<bool>("TechOnly", true),
            new Setting<bool>("DelayedJumpRelease", true),
            new Setting<Time>("ReleaseDelay", 0.2f, 0f, 1f)
        }),

        new TweakState(Tweak.BackboostProtection, new SettingContainer {
            new Setting<Time>("EarlyBackboostTiming", 0.35f, 0f, 1f),
            new Setting<Time>("LateBackboostTiming", 0.1f, 0f, 1f)
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
            new Setting<int>("CustomAcceleration", 10, 0, 250),
            new Setting<bool>("BufferableMaxjumps", true)
        }),

        new TweakState(Tweak.CornerWaveLeniency),

        new TweakState(Tweak.CrouchOnBonk),

        new TweakState(Tweak.CustomBufferTime, new SettingContainer {
            new Setting<Time>("JumpBufferTime", 4, 0f, 0.25f),
            new Setting<Time>("DashBufferTime", 4, 0f, 0.25f),
            new Setting<Time>("DemoDashBufferTime", 4, 0f, 0.25f)
        }), 

        new TweakState(Tweak.CustomDashbounceTiming, new SettingContainer {
            new Setting<Time>("Timing", 0.1f, 0f, 0.5f)
        }),

        new TweakState(Tweak.CustomSnapDownDistance, FlexDistanceSettings()),

        new TweakState(Tweak.DashCDIgnoreFFrames),

        new TweakState(Tweak.DelayedClimbtrigger, new SettingContainer {
            new Setting<Time>("MaxDelay", 0.25f, 0f, 0.5f)
        }),

        new TweakState(Tweak.DirectionalReleaseProtection, new SettingContainer {
            new Setting<LeniencyHelperModule.Dirs>("DashDir", LeniencyHelperModule.Dirs.Down),
            new Setting<LeniencyHelperModule.Dirs>("JumpDir", LeniencyHelperModule.Dirs.None),
            new Setting<Time>("ProtectionTime", 0.1f, 0f, 0.25f),
            new Setting<bool>("AffectFeathers", false),
            new Setting<bool>("AffectSuperdashes", false)
        }),

        new TweakState(Tweak.DisableBackboost),

        new TweakState(Tweak.DisableForcemovedTech),

        new TweakState(Tweak.DynamicCornerCorrection, new SettingContainer {
            new Setting<Time>("FloorCorrectionTiming", 0.05f, 0f, 0.25f),
            new Setting<Time>("WallCorrectionTiming", 0.05f, 0f, 0.25f)
        }),

        new TweakState(Tweak.DynamicWallLeniency, new SettingContainer {
            new Setting<Time>("Timing", 0.05f, 0f, 0.25f)
        }),

        new TweakState(Tweak.ExtendBufferOnFreezeAndPickup, new SettingContainer {
            new Setting<bool>("OnFreeze", true),
            new Setting<bool>("OnPickup", false)
        }),

        new TweakState(Tweak.ExtendDashAttackOnPickup, new SettingContainer {
            new Setting<Time>("ExtendTime", 0.1f, 0f, 0.5f)
        }),

        new TweakState(Tweak.ForceCrouchDemodash),

        new TweakState(Tweak.GultraCancel, new SettingContainer {
            new Setting<Time>("MaxCancelDelay", 0.08f, 0f, 0.5f) // no max
        }),

        new TweakState(Tweak.IceWallIncreaseWallLeniency, new SettingContainer {
            new Setting<int>("ExtraWalljumpDistance", 4, 0, 12)
        }),

        new TweakState(Tweak.InstantAcceleratedJumps),

        new TweakState(Tweak.LateReverses, new SettingContainer {
            new Setting<Time>("ReverseTiming", 0.08f, 0f, 0.25f)
        }),

        new TweakState(Tweak.ManualDreamhyperLeniency),

        new TweakState(Tweak.NoFailedTech, new SettingContainer {
            new Setting<Time>("ProtectedTechTime", 0.1f, 0f, 0.5f) // no max
        }),

        new TweakState(Tweak.RefillDashInCoyote, new SettingContainer {
            new Setting<Time>("RefillCoyoteTime", 0.05f, 0f, 0.2f)
        }),

        new TweakState(Tweak.RemoveDBlockCCorrection),

        new TweakState(Tweak.RespectInputOrder, new SettingContainer {
            new Setting<bool>("AffectGrab", false)
        }),

        new TweakState(Tweak.RetainSpeedCornerboost, new SettingContainer {
            new Setting<Time>("MaxRetainTime", 0.1f, 0f, 0.5f) // no max
        }),

        new TweakState(Tweak.SolidBlockboostProtection, new SettingContainer {
            new Setting<Time>("MaxSaveTime", 0.1f, 0f, 0.5f)
        }),
         
        new TweakState(Tweak.SuperdashSteeringProtection, new SettingContainer {
            new Setting<bool>("CloseAngleRestriction", true)
        }),

        new TweakState(Tweak.SuperOverWalljump),

        new TweakState(Tweak.WallAttraction, FlexDistanceSettings(defaultStatic: 6)),

        new TweakState(Tweak.WallCoyoteFrames, new SettingContainer {
            new Setting<Time>("WallCoyoteTime", 4, 0, 0.1f)
        })
    };  }


    public static void ResetPlayerSettings()
    {
        foreach (AbstractSetting setting in LeniencyHelperModule.TweakList.
            Where(t => t.HasSettings()).SelectMany(tweak => Tweaks[tweak].Settings))
        {
            setting.Reset(SettingSource.Player);
        }
    }


    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.ctor += DisableTriggerTweaksHook;
        IL.Celeste.LevelLoader.ctor += DisableTriggerTweaksHook;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.ctor -= DisableTriggerTweaksHook;
        IL.Celeste.LevelLoader.ctor -= DisableTriggerTweaksHook;
    }

    static void DisableTriggerTweaksHook(ILContext il)
    {
        new ILCursor(il).EmitDelegate(DisableTriggerTweaks);
        static void DisableTriggerTweaks() => Tweaks.ForEach(state => state.Set(null, SettingSource.Trigger));
    }
}