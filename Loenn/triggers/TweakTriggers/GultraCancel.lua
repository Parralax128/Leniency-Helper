local trigger = {}

trigger.name = "LeniencyHelper/Triggers/GultraCancel"
trigger.triggerText = "Gultra Cancel Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Gultra Cancel Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return trigger