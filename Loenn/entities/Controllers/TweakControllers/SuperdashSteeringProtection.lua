local controller = {}

controller.name = "LeniencyHelper/Controllers/SuperdashSteeringProtection"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Superdash Steering protection Controller",
        data = {
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller