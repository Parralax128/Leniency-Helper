local controller = {}

controller.name = "LeniencyHelper/Controllers/LateReverses"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Late Reverses Controller",
        data = {
            RedirectTiming = 0.08,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller