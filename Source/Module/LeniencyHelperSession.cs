using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using static Celeste.Mod.LeniencyHelper.Triggers.InputRequiresBlockboostTrigger;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Celeste.Mod.LeniencyHelper.Module;

public class LeniencyHelperSession : EverestModuleSession
{
    #region temp vars

    
    public int wjDistR { get; set; } = 3;
    public int wjDistL { get; set; } = 3;

    //AutoSlowfall
    public float jumpReleaseTimer = 0f;
    public bool inTechState = false;
    
    //BackboostProtection
    public Facings lastFacing { get; set; } = Facings.Right;
    public float rightTimer { get; set; } = 0f;
    public float leftTimer { get; set; } = 0f;
    public float speedXbeforePickup { get; set; } = 0f;

    //BufferableExtends
    public float dashTimer { get; set; } = 0f;
    public bool dreamDashEnded { get; set; } = false;

    //CustomDashbounceTiming
    public float? dashbounceTimer { get; set; } = null;
    public bool? canDashbounce { get; set; } = null;
    public float varJumpTime { get; set; } = 0.25f;
    public float dashDuration { get; set; } = 0.2f;

    //DashCDIgnoreFFrames
    public Player modifiedPlayer { get; set; }
    public bool useOrigFreeze { get; set; } = false;

    //DirectionalReleaseProtection
    public float featherDirDelay { get; set; } = 0.05f;

    //DynamicCornerCorrection
    public Vector2 cornerCorrection { get; set; } = Vector2.Zero;

    //ExtendBufferOnFreezeAndPickup
    public float pickupDelay { get; set; } = 0.016f;
    public float pickupTimeLeft { get; set; } = 0f;
    public int prevFrameState { get; set; } = 0;
    public bool jumpExtended { get; set; } = false;
    public bool dashExtended { get; set; } = false;
    public bool demoExtended { get; set; } = false;

    // GultraCancel
    public Vector2? savedSpeed { get; set; } = null;
    public float cancelTimer = 0f;

    //InstantClimbHop
    public bool movedDown { get; set; } = false;
    public Solid savedClimbHopSolid { get; set; }

    //NoFailedTech
    public bool dashCrouched { get; set; } = false;
    public float protectedDashAttackTimer { get; set; } = 0f;
    public bool downDiag { get; set; } = false;

    //RefillDashInCoyote
    public bool artificialChecking { get; set; } = false;

    //RetainSpeedCornerboost
    public float retainCbSpeed { get; set; }
    public float retainCbSpeedTimer { get; set; }

    // LateReverses
    public float redirectTimer { get; set; } = 0f;
    public Facings prevFrameFacing { get; set; }
    public float redirectSpeed { get; set; }

    #endregion


    #region triggers
    //ClearBlockBoostTrigger
    public bool clearBlockBoostActivated { get; set; } = false;

    //DisalbeAirMovementTrigger
    public bool airMovementDisabled { get; set; } = false;

    //InputRequiresBlockboostTrigger
    public List<BindInfo> BindList { get; set; } = new List<BindInfo>();
    public Vector2 playerLiftboost { get; set; } = Vector2.Zero;
    #endregion
}