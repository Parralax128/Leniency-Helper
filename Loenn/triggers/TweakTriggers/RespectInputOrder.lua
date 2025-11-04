local trigger = {}

trigger.name = "LeniencyHelper/Triggers/RespectInputOrder"
trigger.triggerText = "Respect Input Order Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Respect Input Order Trigger",
        data = { 
            AffectGrab = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return trigger