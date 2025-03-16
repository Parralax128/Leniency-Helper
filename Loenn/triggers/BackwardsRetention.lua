local BackwardsRetention = {}

BackwardsRetention.name = "LeniencyHelper/Triggers/BackwardsRetention"
BackwardsRetention.triggerText = "Backwards Retention Trigger"
BackwardsRetention.depth = -1000000
BackwardsRetention.placements = {
    {
        name = "Backwards Retention Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return BackwardsRetention