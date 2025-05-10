local trigger = {}

trigger.name = "LeniencyHelper/Triggers/DirectionalReleaseProtection"
trigger.triggerText = "Directional Release Protection Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Directional Release Protection Trigger",
        data = { 
            BufferTime = 0.1,
            FreezeFramesExtendBufferTimer = false,
            ProtectedDashDirections = "Down",
            ProtectedJumpDirections = "None",
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

trigger.fieldInformation = {
    ProtectedDashDirections = {fieldType="string",options={"Up","Down","Left","Right","All"},editable=false},
    ProtectedJumpDirections = {fieldType="string",options={"Up","Down","Left","Right","All"},editable=false}
}

return trigger