local CustomBufferTime = {}

CustomBufferTime.name = "LeniencyHelper/Triggers/CustomBufferTime"
CustomBufferTime.triggerText = "Custom BufferTime Trigger"
CustomBufferTime.depth = -1000000
CustomBufferTime.placements = {
    {
        name = "Custom BufferTime Trigger",
        data = { 
            JumpBufferTime = 0.08,
            DashBufferTime = 0.08,
            DemoBufferTime = 0.08,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return CustomBufferTime