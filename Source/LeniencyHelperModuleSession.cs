using Microsoft.Xna.Framework;
using Celeste.Mod.LeniencyHelper.Components;
using System;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using static Celeste.Mod.LeniencyHelper.Triggers.InputRequiresBlockboostTrigger;
using System.Collections.Generic;
using Celeste.Mod.LeniencyHelper.Triggers;
using Celeste.Mod.LeniencyHelper.Tweaks;

namespace Celeste.Mod.LeniencyHelper;

public class LeniencyHelperModuleSession : EverestModuleSession {
    public object GetValue(string name)
    {
        try { return this.GetType().GetProperty(name).GetValue(this); }
        catch { Log($"Session failed at getting {name}"); return null; }
    }
    public void SetValue(string name, object value)
    {
        try { this.GetType().GetProperty(name).SetValue(this, value); }
        catch { Log($"Session failed at settings {name}"); }
    }

    public Dictionary<string, bool> TweaksByMap { get; set; } = EmptyDict();
    public static Dictionary<string, bool> EmptyDict()
    {
        Dictionary<string, bool> result = new Dictionary<string, bool>();
        foreach (string tweak in LeniencyHelperModule.Tweaks)
        {
            result.Add(tweak, false);
        }
        return result;
    }

    public Dictionary<string, bool> TweaksEnabled
    {
        get { return LeniencyHelperModule.Instance.TweaksEnabled; }
        set { LeniencyHelperModule.Instance.TweaksEnabled = value; }
    }

    public void UpdateTweak(string tweakName)
    {
        if (!TweaksByMap.ContainsKey(tweakName))
        {
            TweaksByMap.Add(tweakName, false);
        }

        TweaksEnabled[tweakName] = LeniencyHelperModule.Settings.TweaksByPlayer[tweakName] is null ?
                TweaksByMap[tweakName] : LeniencyHelperModule.Settings.TweaksByPlayer[tweakName] == true;
    }
    public void SetMapValue(string tweakName, bool mapValue)
    {
        if (!TweaksByMap.ContainsKey(tweakName)) TweaksByMap.Add(tweakName, mapValue);
        else TweaksByMap[tweakName] = mapValue;

        TweaksEnabled[tweakName] = LeniencyHelperModule.Settings.TweaksByPlayer[tweakName] is null ?
                TweaksByMap[tweakName] : LeniencyHelperModule.Settings.TweaksByPlayer[tweakName] == true;
    }


    #region tweak vars

    //BufferableClimbtrigger
    public Solid climbSolid { get; set; }
    public bool mapForceWaitForRefill { get; set; } = false;
    public bool mapOnlyOnClimbjumps { get; set; } = false;


    //BufferableExtends
    public float dashTimer { get; set; } = 0f;
    public int prevFrameStateBufferableExtends { get; set; }

    //ConsistentTheoSpinnerBounce
    public ConsistentTheoSpinnerBounceTrigger.BounceDirections spinnerBounceDir { get; set; }
        = ConsistentTheoSpinnerBounceTrigger.BounceDirections.All;
    public bool consistentSpinnerBounceEnabled { get; set; } = false;

    //ConsistentDashOnDBlockExit
    public bool mapResetDashCDonLeave { get; set; } = true;
    public bool dreamDashEnded { get; set; } = false;
    public bool justEntered { get; set; } = false;


    //CornerWaveLeniency
    public bool forceForceRide { get; set; } = false;
    public int wjDist { get; set; } = 3;
    public bool mapAllowSpikedFloor { get; set; } = false;

    //CustomBufferTime
    public Dictionary<Inputs, float> defaultBuffers { get; set; } = CustomBufferTime.GetVanillaBuffers();
    public Dictionary<Inputs, float> mapBuffers { get; set; } = new Dictionary<Inputs, float>
    {
        { Inputs.Jump, 0.08f },
        { Inputs.Dash, 0.08f },
        { Inputs.Demo, 0.08f }
    };
    public bool mapCountBufferTimeInFrames { get; set; } = false;

    //DashCDIgnoreFFrames
    public Player modifiedPlayer { get; set; }
    public bool useOrigFreeze { get; set; } = false;

    //DirectionalReleaseProtection
    public Dirs mapDashDir { get; set; } = Dirs.Down;
    public Dirs mapJumpDir { get; set; } = Dirs.None;
    public float mapDirectionalBufferTime { get; set; } = 0.05f;
    public bool mapExtendByFFrames { get; set; } = true;
    public bool mapCountProtectionTimeInFrames { get; set; } = false;

    //DynamicCornerCorrection
    public float mapFloorCorrectionTiming { get; set; } = 0.1f;
    public float mapWallCorrectionTiming { get; set; } = 0.05f;
    public bool mapCcorectionTimingInFrames { get; set; } = false;
    public int savedCornerCorrection { get; set; } = 4;

    //DynamicWallLeniency
    public bool mapCountWallTimingInFrames { get; set; } = false;
    public float mapWallLeniencyTiming { get; set; } = 0.05f;

    //ExtendBufferOnFreezeAndPickup
    public float pickupDelay { get; set; } = 0.016f;

    //FFramesExtendBuffer
    public float pickupTimeLeft { get; set; } = 0f;
    public int prevFrameState { get; set; } = 0;
    public bool jumpExtended { get; set; } = false;
    public bool dashExtended { get; set; } = false;
    public bool demoExtended { get; set; } = false;
    public bool mapExtendBufferOnFreeze { get; set; } = true;
    public bool mapExtendBufferOnPickup { get; set; } = true;

    //FrozenReverses
    public On.Celeste.Player.orig_SuperJump origSuperJump { get; set; }
    public bool playerDucked { get; set; } = false;
    public float mapReversedFreezeTime { get; set; } = 2f;
    public bool mapCountReversedInFrames { get; set; } = true;

    //IceWallIncreaseWallLeniency
    public int mapIceWJLeniency { get; set; } = 3;

    //InstantClimbHop
    public bool movedDown { get; set; } = false;
    public Solid savedClimbHopSolid { get; set; }

    //NoFailedTech
    public bool dashCrouched { get; set; } = false;
    public float protectedDashAttackTimer { get; set; } = 0f;

    //RefillDashInCoyote
    public bool artificialChecking { get; set; } = false;
    public bool canRefill { get; set; } = false;
    public RefillCoyoteComponent RCcomponent { get; set; } = new RefillCoyoteComponent();
    public float mapRefillCoyoteTime { get; set; } = 0.05f;
    public bool mapCountRefillCoyoteTimeInFrames { get; set; } = false;

    //RetainSpeedCornerboost
    public float retainCbSpeed { get; set; }
    public float retainCbSpeedTimer { get; set; }
    public float mapRetainCbSpeedTime { get; set; } = 0.25f;
    public bool mapCountRetainTimeInFrames { get; set; } = false;

    //SuperOverWalljumpPriority
    public float mapBboostSaveTime { get; set; } = 0.1f;
    public bool mapCountSolidBoostSaveTimeInFrames { get; set; } = false;

    //WallAttraction
    public float mapWallApproachTime { get; set; } = 0.05f;
    public bool mapCountAttractionTimeInFrames { get; set; } = false;

    //WallCoyoteFrames
    public WallCoyoteFramesComponent WCFcomponent { get; set; } = new WallCoyoteFramesComponent();
    public float wallCoyoteTimer { get; set; }
    public float prevWallCoyoteTime { get; set; }

    public float mapWallCoyoteTime { get; set; } = 0.1f;
    public bool mapCountWallCoyoteTimeInFrames { get; set; } = false;

    #endregion

    #region triggers

    //ClearBlockBoostTrigger
    public bool clearBlockBoostActivated { get; set; } = false;


    //DisalbeAirMovementTrigger
    public bool airMovementDisabled { get; set; } = false;

    //InputRequiresBlockboostTrigger
    public BindInfo[] BindList = Array.Empty<BindInfo>();
    public Vector2 playerLiftboost { get; set; } = Vector2.Zero;

    #endregion
}