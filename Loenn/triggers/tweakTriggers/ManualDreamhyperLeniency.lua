local trigger = {}

trigger.name = "LeniencyHelper/Triggers/ManualDreamhyperLeniency"
trigger.triggerText = "Manual Dreamhyper leniency Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Manual Dreamhyper leniency Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return trigger