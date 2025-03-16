local WallAttraction = {}

WallAttraction.name = "LeniencyHelper/Triggers/WallAttraction"
WallAttraction.triggerText = "Wall Attraction Trigger"
WallAttraction.depth = -1000000
WallAttraction.placements = {
    {
        name = "Wall Attraction Trigger",
        data = { 
            ApproachTime = 0.085,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return WallAttraction