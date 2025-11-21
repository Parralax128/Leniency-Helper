local controller = {}

controller.name = "LeniencyHelper/Controllers/WallAttraction"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Wall Attraction Controller",
        data = {
            StaticApproachDistance = 3,
            UseDynamicApproachDistance = false,
            ApproachTime = 0.08,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller