local WallAttraction = {}

WallAttraction.name = "LeniencyHelper/Triggers/WallAttraction"
WallAttraction.triggerText = "Wall Attraction Trigger"
WallAttraction.depth = -1000000
WallAttraction.placements = {
    {
        name = "Wall Attraction Trigger",
        data = { 
            StaticApproachDistance = 3,
            UseDynamicApproachDistance = false,
            ApproachTime = 0.08,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return WallAttraction