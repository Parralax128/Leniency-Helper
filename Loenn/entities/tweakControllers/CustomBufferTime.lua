local controller = {}

controller.name = "LeniencyHelper/Controllers/CustomBufferTime"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Custom buffer time Controller",
        data = {
            JumpBufferTime = 0.08,
            DashBufferTime = 0.08,
            DemodashBufferTime = 0.08,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller