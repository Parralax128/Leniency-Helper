local controller = {}

controller.name = "LeniencyHelper/Controllers/DirectionalReleaseProtection"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Directional inputs release protection Controller",
        data = {
            ProtectedDashDirection = Down,
            ProtectedJumpDirection = None,
            ProtectionTime = 0.1,
            CountTimeInFrames = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

return controller