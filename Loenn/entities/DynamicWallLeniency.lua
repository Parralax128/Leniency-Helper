local controller = {}

controller.name = "LeniencyHelper/Controllers/DynamicWallLeniency"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Dynamic wall leniency Controller",
        data = {
            WallLeniencyTiming = 0.05,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller