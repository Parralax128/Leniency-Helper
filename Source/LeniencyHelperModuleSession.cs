using Microsoft.Xna.Framework;
using Celeste.Mod.LeniencyHelper.Components;
using System;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using static Celeste.Mod.LeniencyHelper.Triggers.InputRequiresBlockboostTrigger;
using System.Collections.Generic;
using Celeste.Mod.LeniencyHelper.Tweaks;
using Celeste.Mod.LeniencyHelper.Triggers;
using static Celeste.Mod.LeniencyHelper.SettingMaster;

namespace Celeste.Mod.LeniencyHelper;

public class LeniencyHelperModuleSession : EverestModuleSession {

    public Dictionary<string, TweakState> Tweaks { get; set; } = GetEmptyTweaks();
    private static Dictionary<string, TweakState> GetEmptyTweaks()
    {
        Dictionary<string, TweakState> result = new Dictionary<string, TweakState>();
        foreach (string tweak in tweakList)
        {
            result.Add(tweak, new TweakState(LeniencyHelperModule.Settings.SavedPlayerTweaks[tweak]));
        }
        return result;
    }

    #region temp vars
    //BufferableExtends
    public float dashTimer { get; set; } = 0f;
    public bool dreamDashEnded { get; set; } = false;
    public int wjDist { get; set; } = 3;

    //CustomDashbounceTiming
    public float dashbounceTimer { get; set; } = 0f;

    //DashCDIgnoreFFrames
    public Player modifiedPlayer { get; set; }
    public bool useOrigFreeze { get; set; } = false;

    //DirectionalReleaseProtection
    public Dirs mapDashDir { get; set; } = Dirs.Down;
    public Dirs mapJumpDir { get; set; } = Dirs.None;

    //DynamicCornerCorrection
    public int savedCornerCorrection { get; set; } = 4;

    //ExtendBufferOnFreezeAndPickup
    public float pickupDelay { get; set; } = 0.016f;

    //FFramesExtendBuffer
    public float pickupTimeLeft { get; set; } = 0f;
    public int prevFrameState { get; set; } = 0;
    public bool jumpExtended { get; set; } = false;
    public bool dashExtended { get; set; } = false;
    public bool demoExtended { get; set; } = false;

    //InstantClimbHop
    public bool movedDown { get; set; } = false;
    public Solid savedClimbHopSolid { get; set; }

    //NoFailedTech
    public bool dashCrouched { get; set; } = false;
    public float protectedDashAttackTimer { get; set; } = 0f;

    //RefillDashInCoyote
    public bool artificialChecking { get; set; } = false;
    public RefillCoyoteComponent RCcomponent { get; set; } = new RefillCoyoteComponent();

    //RetainSpeedCornerboost
    public float retainCbSpeed { get; set; }
    public float retainCbSpeedTimer { get; set; }

    //WallCoyoteFrames
    public WallCoyoteFramesComponent WCFcomponent { get; set; } = new WallCoyoteFramesComponent();
    public float wallCoyoteTimer { get; set; }
    public float prevWallCoyoteTime { get; set; }
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