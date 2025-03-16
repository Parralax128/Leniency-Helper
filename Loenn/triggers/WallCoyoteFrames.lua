local WallCoyoteFrames = {}

WallCoyoteFrames.name = "LeniencyHelper/Triggers/WallCoyoteFrames"
WallCoyoteFrames.triggerText = "Wall Coyote-frames Trigger"
WallCoyoteFrames.depth = -1000000
WallCoyoteFrames.placements = {
    {
        name = "Wall Coyote-frames Trigger",
        data = { 
            WallCoyoteTime = 0.065,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return WallCoyoteFrames