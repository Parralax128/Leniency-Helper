local controller = {}

controller.name = "LeniencyHelper/Controllers/DisableForceMove"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Disable forcemove Controller",
        data = {
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller