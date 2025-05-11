local controller = {}

controller.name = "LeniencyHelper/Controllers/NoFailedTech"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "No failed tech Controller",
        data = {
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller