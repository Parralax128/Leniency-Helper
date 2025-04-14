local controller = {}

controller.name = "LeniencyHelper/Controllers/ConsistentDashOnDBlockExit"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Consistent dash on dreamblock exit Controller",
        data = {
            ResetDashCooldown = true,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller