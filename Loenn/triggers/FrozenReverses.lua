local FrozenReverses = {}

FrozenReverses.name = "LeniencyHelper/Triggers/FrozenReverses"
FrozenReverses.triggerText = "Frozen Reverses Trigger"
FrozenReverses.depth = -1000000
FrozenReverses.placements = {
    {
        name = "Frozen Reverses Trigger",
        data = { 
            FreezeTime = 0.034,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return FrozenReverses