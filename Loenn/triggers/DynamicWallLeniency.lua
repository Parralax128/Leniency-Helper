local DynamicWallLeniency = {}

DynamicWallLeniency.name = "LeniencyHelper/Triggers/DynamicWallLeniency"
DynamicWallLeniency.triggerText = "Dynamic Wall-leniency Trigger"
DynamicWallLeniency.depth = -1000000
DynamicWallLeniency.placements = {
    {
        name = "Dynamic Wall-leniency Trigger",
        data = {
            TimingWindow = 0.1,
            CountInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return DynamicWallLeniency