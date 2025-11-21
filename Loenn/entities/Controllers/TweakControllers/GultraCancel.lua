local controller = {}

controller.name = "LeniencyHelper/Controllers/GultraCancel"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Gultra Cancel Controller",
        data = {
            CancelTime = 0.07,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller