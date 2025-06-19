local controller = {}

controller.name = "LeniencyHelper/Controllers/CrouchOnBonk"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Crouch on Bonk Controller",
        data = {
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller