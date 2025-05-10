local DisableBackboost = {}

DisableBackboost.name = "LeniencyHelper/Triggers/DisableBackboost"
DisableBackboost.triggerText = "Disable Backboost Trigger"
DisableBackboost.depth = -1000000
DisableBackboost.placements = {
    {
        name = "Disable Backboost Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return DisableBackboost