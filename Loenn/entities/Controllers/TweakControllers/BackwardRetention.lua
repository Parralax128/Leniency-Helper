local controller = {}

controller.name = "LeniencyHelper/Controllers/BackwardsRetention"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Backwards Retention Controller",
        data = { 
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller