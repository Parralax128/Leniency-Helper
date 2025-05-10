local controller = {}

controller.name = "LeniencyHelper/Controllers/AutoSlowfall"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Auto Slowfall Controller",
        data = { 
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller