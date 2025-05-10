local trigger = {}

trigger.name = "LeniencyHelper/Triggers/SuperdashSteeringProtection"
trigger.triggerText = "Superdash Steering protection Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Superdash Steering protection Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return trigger