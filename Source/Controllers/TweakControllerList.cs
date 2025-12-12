using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.Controllers;

[CustomEntity("LeniencyHelper/Controllers/AutoSlowfall")]
class AutoSlowfallController : GenericTweakController
{
    public AutoSlowfallController(EntityData data, Vector2 offset) : base(data, offset, Tweak.AutoSlowfall) { }
}


[CustomEntity("LeniencyHelper/Controllers/BackboostProtection")]
class BackboostProtectionController : GenericTweakController
{
    public BackboostProtectionController(EntityData data, Vector2 offset) : base(data, offset, Tweak.BackboostProtection) { }
}


[CustomEntity("LeniencyHelper/Controllers/BackwardsRetention")]
class BackwardsRetentionController : GenericTweakController
{
    public BackwardsRetentionController(EntityData data, Vector2 offset) : base(data, offset, Tweak.BackwardsRetention) { }
}


[CustomEntity("LeniencyHelper/Controllers/BufferableClimbtrigger")]
class BufferableClimbtriggerController : GenericTweakController
{
    public BufferableClimbtriggerController(EntityData data, Vector2 offset) : base(data, offset, Tweak.BufferableClimbtrigger) { }
}


[CustomEntity("LeniencyHelper/Controllers/BufferableExtends")]
class BufferableExtendsController : GenericTweakController
{
    public BufferableExtendsController(EntityData data, Vector2 offset) : base(data, offset, Tweak.BufferableExtends) { }
}


[CustomEntity("LeniencyHelper/Controllers/ConsistentDashOnDBlockExit")]
class ConsistentDashOnDBlockExitController : GenericTweakController
{
    public ConsistentDashOnDBlockExitController(EntityData data, Vector2 offset) : base(data, offset, Tweak.ConsistentDashOnDBlockExit) { }
}


[CustomEntity("LeniencyHelper/Controllers/ConsistentWallboosters")]
class ConsistentWallboostersController : GenericTweakController
{
    public ConsistentWallboostersController(EntityData data, Vector2 offset) : base(data, offset, Tweak.ConsistentWallboosters) { }
}


[CustomEntity("LeniencyHelper/Controllers/CornerWaveLeniency")]
class CornerWaveLeniencyController : GenericTweakController
{
    public CornerWaveLeniencyController(EntityData data, Vector2 offset) : base(data, offset, Tweak.CornerWaveLeniency) { }
}


[CustomEntity("LeniencyHelper/Controllers/CrouchOnBonk")]
class CrouchOnBonkController : GenericTweakController
{
    public CrouchOnBonkController(EntityData data, Vector2 offset) : base(data, offset, Tweak.CrouchOnBonk) { }
}


[CustomEntity("LeniencyHelper/Controllers/CustomBufferTime")]
class CustomBufferTimeController : GenericTweakController
{
    public CustomBufferTimeController(EntityData data, Vector2 offset) : base(data, offset, Tweak.CustomBufferTime) { }
}


[CustomEntity("LeniencyHelper/Controllers/CustomDashbounceTiming")]
class CustomDashbounceTimingController : GenericTweakController
{
    public CustomDashbounceTimingController(EntityData data, Vector2 offset) : base(data, offset, Tweak.CustomDashbounceTiming) { }
}


[CustomEntity("LeniencyHelper/Controllers/CustomSnapDownDistance")]
class CustomSnapDownDistanceController : GenericTweakController
{
    public CustomSnapDownDistanceController(EntityData data, Vector2 offset) : base(data, offset, Tweak.CustomSnapDownDistance) { }
}


[CustomEntity("LeniencyHelper/Controllers/DashCDIgnoreFFrames")]
class DashCDIgnoreFFramesController : GenericTweakController
{
    public DashCDIgnoreFFramesController(EntityData data, Vector2 offset) : base(data, offset, Tweak.DashCDIgnoreFFrames) { }
}


[CustomEntity("LeniencyHelper/Controllers/DelayedClimbtrigger")]
class DelayedClimbtriggerController : GenericTweakController
{
    public DelayedClimbtriggerController(EntityData data, Vector2 offset) : base(data, offset, Tweak.DelayedClimbtrigger) { }
}


[CustomEntity("LeniencyHelper/Controllers/DirectionalReleaseProtection")]
class DirectionalReleaseProtectionController : GenericTweakController
{
    public DirectionalReleaseProtectionController(EntityData data, Vector2 offset) : base(data, offset, Tweak.DirectionalReleaseProtection) { }
}


[CustomEntity("LeniencyHelper/Controllers/DisableBackboost")]
class DisableBackboostController : GenericTweakController
{
    public DisableBackboostController(EntityData data, Vector2 offset) : base(data, offset, Tweak.DisableBackboost) { }
}


[CustomEntity("LeniencyHelper/Controllers/DisableForcemovedTech")]
class DisableForcemovedTechController : GenericTweakController
{
    public DisableForcemovedTechController(EntityData data, Vector2 offset) : base(data, offset, Tweak.DisableForcemovedTech) { }
}


[CustomEntity("LeniencyHelper/Controllers/DynamicCornerCorrection")]
class DynamicCornerCorrectionController : GenericTweakController
{
    public DynamicCornerCorrectionController(EntityData data, Vector2 offset) : base(data, offset, Tweak.DynamicCornerCorrection) { }
}


[CustomEntity("LeniencyHelper/Controllers/DynamicWallLeniency")]
class DynamicWallLeniencyController : GenericTweakController
{
    public DynamicWallLeniencyController(EntityData data, Vector2 offset) : base(data, offset, Tweak.DynamicWallLeniency) { }
}


[CustomEntity("LeniencyHelper/Controllers/ExtendBufferOnFreezeAndPickup")]
class ExtendBufferOnFreezeAndPickupController : GenericTweakController
{
    public ExtendBufferOnFreezeAndPickupController(EntityData data, Vector2 offset) : base(data, offset, Tweak.ExtendBufferOnFreezeAndPickup) { }
}


[CustomEntity("LeniencyHelper/Controllers/ExtendDashAttackOnPickup")]
class ExtendDashAttackOnPickupController : GenericTweakController
{
    public ExtendDashAttackOnPickupController(EntityData data, Vector2 offset) : base(data, offset, Tweak.ExtendDashAttackOnPickup) { }
}


[CustomEntity("LeniencyHelper/Controllers/ForceCrouchDemodash")]
class ForceCrouchDemodashController : GenericTweakController
{
    public ForceCrouchDemodashController(EntityData data, Vector2 offset) : base(data, offset, Tweak.ForceCrouchDemodash) { }
}


[CustomEntity("LeniencyHelper/Controllers/GultraCancel")]
class GultraCancelController : GenericTweakController
{
    public GultraCancelController(EntityData data, Vector2 offset) : base(data, offset, Tweak.GultraCancel) { }
}


[CustomEntity("LeniencyHelper/Controllers/IceWallIncreaseWallLeniency")]
class IceWallIncreaseWallLeniencyController : GenericTweakController
{
    public IceWallIncreaseWallLeniencyController(EntityData data, Vector2 offset) : base(data, offset, Tweak.IceWallIncreaseWallLeniency) { }
}


[CustomEntity("LeniencyHelper/Controllers/InstantAcceleratedJumps")]
class InstantAcceleratedJumpsController : GenericTweakController
{
    public InstantAcceleratedJumpsController(EntityData data, Vector2 offset) : base(data, offset, Tweak.InstantAcceleratedJumps) { }
}


[CustomEntity("LeniencyHelper/Controllers/LateReverses")]
class LateReversesController : GenericTweakController
{
    public LateReversesController(EntityData data, Vector2 offset) : base(data, offset, Tweak.LateReverses) { }
}


[CustomEntity("LeniencyHelper/Controllers/ManualDreamhyperLeniency")]
class ManualDreamhyperLeniencyController : GenericTweakController
{
    public ManualDreamhyperLeniencyController(EntityData data, Vector2 offset) : base(data, offset, Tweak.ManualDreamhyperLeniency) { }
}


[CustomEntity("LeniencyHelper/Controllers/NoFailedTech")]
class NoFailedTechController : GenericTweakController
{
    public NoFailedTechController(EntityData data, Vector2 offset) : base(data, offset, Tweak.NoFailedTech) { }
}


[CustomEntity("LeniencyHelper/Controllers/RefillDashInCoyote")]
class RefillDashInCoyoteController : GenericTweakController
{
    public RefillDashInCoyoteController(EntityData data, Vector2 offset) : base(data, offset, Tweak.RefillDashInCoyote) { }
}


[CustomEntity("LeniencyHelper/Controllers/RemoveDBlockCCorection")]
class RemoveDBlockCCorectionController : GenericTweakController
{
    public RemoveDBlockCCorectionController(EntityData data, Vector2 offset) : base(data, offset, Tweak.RemoveDBlockCCorrection) { }
}


[CustomEntity("LeniencyHelper/Controllers/RespectInputOrder")]
class RespectInputOrderController : GenericTweakController
{
    public RespectInputOrderController(EntityData data, Vector2 offset) : base(data, offset, Tweak.RespectInputOrder) { }
}


[CustomEntity("LeniencyHelper/Controllers/RetainSpeedCornerboost")]
class RetainSpeedCornerboostController : GenericTweakController
{
    public RetainSpeedCornerboostController(EntityData data, Vector2 offset) : base(data, offset, Tweak.RetainSpeedCornerboost) { }
}


[CustomEntity("LeniencyHelper/Controllers/SolidBlockboostProtection")]
class SolidBlockboostProtectionController : GenericTweakController
{
    public SolidBlockboostProtectionController(EntityData data, Vector2 offset) : base(data, offset, Tweak.SolidBlockboostProtection) { }
}


[CustomEntity("LeniencyHelper/Controllers/SuperdashSteeringProtection")]
class SuperdashSteeringProtectionController : GenericTweakController
{
    public SuperdashSteeringProtectionController(EntityData data, Vector2 offset) : base(data, offset, Tweak.SuperdashSteeringProtection) { }
}


[CustomEntity("LeniencyHelper/Controllers/SuperOverWalljump")]
class SuperOverWalljumpController : GenericTweakController
{
    public SuperOverWalljumpController(EntityData data, Vector2 offset) : base(data, offset, Tweak.SuperOverWalljump) { }
}


[CustomEntity("LeniencyHelper/Controllers/WallAttraction")]
class WallAttractionController : GenericTweakController
{
    public WallAttractionController(EntityData data, Vector2 offset) : base(data, offset, Tweak.WallAttraction) { }
}


[CustomEntity("LeniencyHelper/Controllers/WallCoyoteFrames")]
class WallCoyoteFramesController : GenericTweakController
{
    public WallCoyoteFramesController(EntityData data, Vector2 offset) : base(data, offset, Tweak.WallCoyoteFrames) { }
}