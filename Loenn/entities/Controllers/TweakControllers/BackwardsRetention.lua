local controller = {}

controller.name = "LeniencyHelper/Controllers/ThrowableCeilingBumpController"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Throwable ceiling bump Controller",
        data = { 
            DisableGroundFriction = false,
            EntityList = "Glider, TheoCrystal",
            StopFlag = ""
        }
    }
}

return controller