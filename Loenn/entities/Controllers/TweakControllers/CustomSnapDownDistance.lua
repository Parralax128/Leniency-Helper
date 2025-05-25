local controller = {}

controller.name = "LeniencyHelper/Controllers/CustomSnapDownDistance"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Custom Snap down distance Controller",
        data = { 
            StaticSnapDownDistance = 3,
            DynamicSnapDownDistance = true,
            SnapDownTiming = 0.05,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller