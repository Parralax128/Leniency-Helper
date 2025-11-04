local controller = {}

controller.name = "LeniencyHelper/Controllers/NoFailedTech"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "No Failed Tech Controller",
        data = {
            ProtectedTechTime = 0.1,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller