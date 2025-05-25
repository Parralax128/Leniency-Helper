local trigger = {}

trigger.name = "LeniencyHelper/Triggers/ConsistentWallboosters"
trigger.triggerText = "Backwards Retention Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Consistent Wallboosters Trigger",
        data = { 
            CustomAcceleration = 10,
            InstantAcceleration = false,
            ConsistentBlockboost = true,
            BufferableMaxJumps = true,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return trigger