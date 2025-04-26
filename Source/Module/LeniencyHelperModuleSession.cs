using Microsoft.Xna.Framework;
using Celeste.Mod.LeniencyHelper.Components;
using System;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using static Celeste.Mod.LeniencyHelper.Triggers.InputRequiresBlockboostTrigger;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace Celeste.Mod.LeniencyHelper.Module;

public class LeniencyHelperModuleSession : EverestModuleSession
{

    public LeniencyHelperModuleSession()
    {
        TriggerTweaks = TweakList.ToDictionary(tweak => tweak, tweak => false);
        ControllerTweaks = TweakList.ToDictionary(tweak => tweak, tweak => false);
        UseController = TweakList.ToDictionary(tweak => tweak, tweak => false);

        TriggerSettings = new SettingList();
        ControllerSettings = new SettingList();
    }

    public Dictionary<string, bool> TriggerTweaks { get; set; } = TweakList.ToDictionary(tweak => tweak, tweak => false);
    public Dictionary<string, bool> ControllerTweaks { get; set; } = TweakList.ToDictionary(tweak => tweak, tweak => false);
    public Dictionary<string, bool> UseController { get; set; } = TweakList.ToDictionary(tweak => tweak, tweak => false);

    public SettingList TriggerSettings { get; set; } = new SettingList();
    public SettingList ControllerSettings { get; set; } = new SettingList();


    #region temp vars
    public int wjDistR { get; set; } = 3;
    public int wjDistL { get; set; } = 3;


    //BufferableExtends
    public float dashTimer { get; set; } = 0f;
    public bool dreamDashEnded { get; set; } = false;

    //CustomDashbounceTiming
    public float dashbounceTimer { get; set; } = 0f;

    //DashCDIgnoreFFrames
    public Player modifiedPlayer { get; set; }
    public bool useOrigFreeze { get; set; } = false;

    //DynamicCornerCorrection
    public Vector2 cornerCorrection { get; set; } = Vector2.Zero;

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

    //RetainSpeedCornerboost
    public float retainCbSpeed { get; set; }
    public float retainCbSpeedTimer { get; set; }

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