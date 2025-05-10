local DynamicCornerCorrection = {}

DynamicCornerCorrection.name = "LeniencyHelper/Triggers/DynamicCornerCorrection"
DynamicCornerCorrection.triggerText = "Dynamic Corner-correction Trigger"
DynamicCornerCorrection.depth = -1000000
DynamicCornerCorrection.placements = {
    {
        name = "Dynamic Corner-correction Trigger",
        data = {
            FloorCorrectionTiming = 0.1,
            WallCorrectionTiming = 0.05,
            CountInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return DynamicCornerCorrection