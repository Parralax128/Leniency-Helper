local controller = {}

controller.name = "LeniencyHelper/Controllers/ConsistentWallboosters"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Consistent Wallboosters Controller",
        data = { 
            CustomAcceleration = 10,
            InstantAcceleration = false,
            ConsistentBlockboost = true,
            BufferableMaxJumps = true,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller