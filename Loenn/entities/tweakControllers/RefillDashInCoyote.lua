local controller = {}

controller.name = "LeniencyHelper/Controllers/RefillDashInCoyote"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Refill dash in-coyote Controller",
        data = {
            RefillTime = 0.05,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller