local CornerWaveLeniency = {}

CornerWaveLeniency.name = "LeniencyHelper/Triggers/CornerWaveLeniency"
CornerWaveLeniency.triggerText = "Corner Wave Leniency Trigger"
CornerWaveLeniency.depth = -1000000
CornerWaveLeniency.placements = {
    {
        name = "Corner Wave Leniency Trigger",
        data = { 
            AllowSpikedFloor = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return CornerWaveLeniency