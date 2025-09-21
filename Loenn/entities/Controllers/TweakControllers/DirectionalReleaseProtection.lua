local controller = {}

controller.name = "LeniencyHelper/Controllers/DirectionalReleaseProtection"
controller.depth = -1000000
controller.texture = "objects/LeniencyHelper/Controllers/genericController"
controller.placements = {
    {
        name = "Directional inputs release protection Controller",
        data = {
            ProtectedDashDirection = "Down",
            ProtectedJumpDirection = "None",
            ProtectionTime = 0.1,
            CountTimeInFrames = false,
            AffectFeathers = false,
            AffectSuperdashes = false,
            StopFlag = "",
            Persistent = true
        }
    }
}

controller.fieldInformation = {
    ProtectedDashDirection = {fieldType="string",options={"Up","Down","Left","Right","All","None"},editable=false},
    ProtectedJumpDirection = {fieldType="string",options={"Up","Down","Left","Right","All","None"},editable=false}
}

return controller