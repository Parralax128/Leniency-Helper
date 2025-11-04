local controller = {}

controller.name = "LeniencyHelper/Controllers/RespectInputOrder"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Respect Input Order Controller",
        data = {
            AffectGrab = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller