local controller = {}

controller.name = "LeniencyHelper/Controllers/BackboostProtection"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Backboost Protection Controller",
        data = { 
            EarlyBackboostTiming = 0.35,
            LateBackboostTiming = 0.1,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller