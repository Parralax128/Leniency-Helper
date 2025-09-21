local trigger = {}

trigger.name = "LeniencyHelper/Triggers/DirectionalReleaseProtection"
trigger.triggerText = "Directional Release Protection Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Directional Release Protection Trigger",
        data = { 
            ProtectedDashDirection = "Down",
            ProtectedJumpDirection = "None",
            ProtectionTime = 0.1,
            CountTimeInFrames = false,
            AffectFeathers = false,
            AffectSuperdashes = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

trigger.fieldInformation = {
    ProtectedDashDirection = {fieldType="string",options={"Up","Down","Left","Right","All","None"},editable=false},
    ProtectedJumpDirection = {fieldType="string",options={"Up","Down","Left","Right","All","None"},editable=false}
}

return trigger