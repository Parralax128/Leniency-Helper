local controller = {}

controller.name = "LeniencyHelper/Controllers/InstantAcceleratedJumps"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Instant accelerated jumps Controller",
        data = {
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller