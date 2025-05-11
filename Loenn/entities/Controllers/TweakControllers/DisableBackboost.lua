local controller = {}

controller.name = "LeniencyHelper/Controllers/DisableBackboost"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Disable backboost Controller",
        data = {
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller