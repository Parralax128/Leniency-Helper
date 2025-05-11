local controller = {}

controller.name = "LeniencyHelper/Controllers/BufferableExtends"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Bufferable extends Controller",
        data = {
            ForceWaitForRefill = false,
            BufferTiming = 0.08,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller