local DisableAirMovementTrigger = {}

DisableAirMovementTrigger.name = "LeniencyHelper/DisableAirMovementTrigger"
DisableAirMovementTrigger.triggerText = "Disable Air-movement Trigger"
DisableAirMovementTrigger.depth = -1000000
DisableAirMovementTrigger.placements = {
    {
        name = "Disable Air-movement Trigger",
        data = {
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return DisableAirMovementTrigger