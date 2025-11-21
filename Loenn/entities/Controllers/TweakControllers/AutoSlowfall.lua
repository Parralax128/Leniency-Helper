local controller = {}

controller.name = "LeniencyHelper/Controllers/AutoSlowfall"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Auto Slowfall Controller",
        data = { 
            TechOnly = false,
            DelayedJumpRelease = false,
            ReleaseDelay = 0.05,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller