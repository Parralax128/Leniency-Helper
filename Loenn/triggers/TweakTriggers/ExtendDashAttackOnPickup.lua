local ExtendDashAttackOnPickup = {}

ExtendDashAttackOnPickup.name = "LeniencyHelper/Triggers/ExtendDashAttackOnPickup"
ExtendDashAttackOnPickup.triggerText = "Extend Dash Attack on Pickup Trigger"
ExtendDashAttackOnPickup.depth = -1000000
ExtendDashAttackOnPickup.placements = {
    {
        name = "Extend Dash Attack on Pickup Trigger",
        data = {
            ExtendTime = 0.1,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return ExtendDashAttackOnPickup