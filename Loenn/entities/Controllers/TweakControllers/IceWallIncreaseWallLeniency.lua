local controller = {}

controller.name = "LeniencyHelper/Controllers/IceWallIncreaseWallLeniency"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Icewall increase Wall-leniency Controller",
        data = {
            AdditionalLeniency = 3,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller