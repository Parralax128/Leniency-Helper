local DashCDIgnoreFFrames = {}

DashCDIgnoreFFrames.name = "LeniencyHelper/Triggers/DashCDIgnoreFFrames"
DashCDIgnoreFFrames.triggerText = "Dash CD ignores Freeze-frames Trigger"
DashCDIgnoreFFrames.depth = -1000000
DashCDIgnoreFFrames.placements = {
    {
        name = "Dash CD ignores Freeze-frames Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return DashCDIgnoreFFrames