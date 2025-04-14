local controller = {}

controller.name = "LeniencyHelper/Controllers/CornerWaveLeniency"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Corner-wave leniency Controller",
        data = {
            AllowSpikedFloor = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller