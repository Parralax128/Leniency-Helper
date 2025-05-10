local controller = {}

controller.name = "LeniencyHelper/Controllers/BufferableClimbtrigger"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Bufferable Climbtrigger Controller",
        data = { 
            InstantUpwardClimbActivation = true,
            ClimbTriggerOnDash = true,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller