local controller = {}

controller.name = "LeniencyHelper/Controllers/ExtendBufferOnFreezeAndPickup"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Extend buffer on freeze and pickup Controller",
        data = {
            ExtendBuffersOnFreezeFrames = true,
            ExtendBuffersOnPickup = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller