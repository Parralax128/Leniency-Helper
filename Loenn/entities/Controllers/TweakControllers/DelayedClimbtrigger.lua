local controller = {}

controller.name = "LeniencyHelper/Controllers/DelayedClimbtrigger"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Delayed Climbtrigger Controller",
        data = { 
            TriggerDelay = 0.25,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller