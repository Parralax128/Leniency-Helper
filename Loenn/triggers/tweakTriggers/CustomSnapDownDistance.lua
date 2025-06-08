local trigger = {}

trigger.name = "LeniencyHelper/Triggers/CustomSnapDownDistance"
trigger.triggerText = "Custom Snap down distance Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Custom Snap down distance Trigger",
        data = { 
            StaticSnapDownDistance = 3,
            DynamicSnapDownDistance = true,
            SnapDownTiming = 0.05,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return trigger