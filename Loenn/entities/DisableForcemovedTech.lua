local controller = {}

controller.name = "LeniencyHelper/Controllers/DisableForcemovedTech"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Disable forcemoved tech Controller",
        data = {
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller