local CustomBufferTime = {}

CustomBufferTime.name = "LeniencyHelper/Triggers/CustomBufferTime"
CustomBufferTime.triggerText = "Custom Buffer time Trigger"
CustomBufferTime.depth = -1000000
CustomBufferTime.placements = {
    {
        name = "Custom Buffer time Trigger",
        data = { 
            JumpBufferTime = 0.08,
            DashBufferTime = 0.08,
            DemodashBufferTime = 0.08,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return CustomBufferTime