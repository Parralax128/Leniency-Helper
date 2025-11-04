local controller = {}

controller.name = "LeniencyHelper/Controllers/CustomDashbounceTiming"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Custom Dashbounce Timing Controller",
        data = {
            Timing = 0.05,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller