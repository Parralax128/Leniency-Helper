local trigger = {}

trigger.name = "LeniencyHelper/Triggers/GultraCancel"
trigger.triggerText = "Gultra Cancel Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Gultra Cancel Trigger",
        data = { 
            CancelTime = 0.07,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return trigger