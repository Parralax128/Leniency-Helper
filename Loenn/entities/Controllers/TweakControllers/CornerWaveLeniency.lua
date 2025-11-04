local controller = {}

controller.name = "LeniencyHelper/Controllers/CornerWaveLeniency"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Corner Wave Leniency Controller",
        data = {
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller