local controller = {}

controller.name = "LeniencyHelper/Controllers/WallCoyoteFrames"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Wall Coyote-frames Controller",
        data = {
            WallCoyoteTime = 0.08,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller