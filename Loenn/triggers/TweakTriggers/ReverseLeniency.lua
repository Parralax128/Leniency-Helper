local trigger = {}

trigger.name = "LeniencyHelper/Triggers/ReverseLeniency"
trigger.triggerText = "Reverse leniency Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Reverse leniency Trigger",
        data = { 
            RedirectTiming = 0.08,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return trigger