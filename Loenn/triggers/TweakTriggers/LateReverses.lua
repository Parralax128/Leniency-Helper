local trigger = {}

trigger.name = "LeniencyHelper/Triggers/LateReverses"
trigger.triggerText = "Late Reverses Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Late Reverses Trigger",
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