local RetainSpeedCornerboost = {}

RetainSpeedCornerboost.name = "LeniencyHelper/Triggers/RetainSpeedCornerboost"
RetainSpeedCornerboost.triggerText = "Retain Speed Cornerboost Trigger"
RetainSpeedCornerboost.depth = -1000000
RetainSpeedCornerboost.placements = {
    {
        name = "Retain Speed Cornerboost Trigger",
        data = { 
            MaxRetentionTime = 0.1,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return RetainSpeedCornerboost