local controller = {}

controller.name = "LeniencyHelper/Controllers/RetainSpeedCornerboost"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Retain Speed Cornerboost Controller",
        data = {
            RetainTime = 0.1,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller