local InstantAcceleratedJumps = {}

InstantAcceleratedJumps.name = "LeniencyHelper/Triggers/InstantAcceleratedJumps"
InstantAcceleratedJumps.triggerText = "Instant-accelerated Jumps Trigger"
InstantAcceleratedJumps.depth = -1000000
InstantAcceleratedJumps.placements = {
    {
        name = "Instant-accelerated Jumps Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return InstantAcceleratedJumps