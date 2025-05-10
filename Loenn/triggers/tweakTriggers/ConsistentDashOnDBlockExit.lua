local ConsistentDashOnDBlockExit = {}

ConsistentDashOnDBlockExit.name = "LeniencyHelper/Triggers/ConsistentDashOnDBlockExit"
ConsistentDashOnDBlockExit.triggerText = "Consistent Dash on Dreamblock exit Trigger"
ConsistentDashOnDBlockExit.depth = -1000000
ConsistentDashOnDBlockExit.placements = {
    {
        name = "Consistent Dash on Dreamblock exit Trigger",
        data = { 
            ResetDashCooldown = true,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return ConsistentDashOnDBlockExit