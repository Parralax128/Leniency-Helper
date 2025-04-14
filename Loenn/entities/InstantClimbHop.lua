local controller = {}

controller.name = "LeniencyHelper/Controllers/InstantClimbHop"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Instant climb hop Controller",
        data = {
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller