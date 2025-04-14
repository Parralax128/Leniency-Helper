local controller = {}

controller.name = "LeniencyHelper/Controllers/SolidBlockboostProtection"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Solid blockboost protection Controller",
        data = {
            ProtectionTime = 0.1,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller