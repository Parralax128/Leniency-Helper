local controller = {}

controller.name = "LeniencyHelper/Controllers/DynamicCornerCorrection"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Dynamic Corner-correction Controller",
        data = {
            FloorCorrectionTiming = 0.05,
            WallCorrectionTiming = 0.05,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller