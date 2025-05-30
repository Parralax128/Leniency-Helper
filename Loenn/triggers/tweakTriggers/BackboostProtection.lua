local trigger = {}

trigger.name = "LeniencyHelper/Triggers/BackboostProtection"
trigger.triggerText = "Backboost Protection Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Backboost Protection Trigger",
        data = { 
            EarlyBackboostTiming = 0.35,
            LateBackboostTiming = 0.1,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return trigger