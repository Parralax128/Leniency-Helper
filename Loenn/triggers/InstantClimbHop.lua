local InstantClimbHop = {}

InstantClimbHop.name = "LeniencyHelper/Triggers/InstantClimbHop"
InstantClimbHop.triggerText = "Instant Climb-Hop Trigger"
InstantClimbHop.depth = -1000000
InstantClimbHop.placements = {
    {
        name = "Instant Climb-Hop Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return InstantClimbHop