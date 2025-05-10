local ExtendBufferOnFreezeAndPickup = {}

ExtendBufferOnFreezeAndPickup.name = "LeniencyHelper/Triggers/ExtendBufferOnFreezeAndPickup"
ExtendBufferOnFreezeAndPickup.triggerText = "Extend Buffer on Freeze and Pickup Trigger"
ExtendBufferOnFreezeAndPickup.depth = -1000000
ExtendBufferOnFreezeAndPickup.placements = {
    {
        name = "Extend Buffer on Freeze and Pickup Trigger",
        data = { 
            ExtendBufferOnFreeze = true,
            ExtendBufferOnPickup = true,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return ExtendBufferOnFreezeAndPickup