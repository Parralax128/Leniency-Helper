local controller = {}

controller.name = "LeniencyHelper/Controllers/ExtendDashAttackOnPickup"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Extend Dash Attack on Pickup Controller",
        data = { 
            ExtendTime = 0.1,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller