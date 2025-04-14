local controller = {}

controller.name = "LeniencyHelper/Controllers/DashCDIgnoreFFrames"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Dash CD ignores freeze-frames Controller",
        data = {
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller