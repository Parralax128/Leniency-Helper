local BufferableExtends = {}

BufferableExtends.name = "LeniencyHelper/Triggers/BufferableExtends"
BufferableExtends.triggerText = "Bufferable Extends Trigger"
BufferableExtends.depth = -1000000
BufferableExtends.placements = {
    {
        name = "Bufferable Extends Trigger",
        data = { 
            ForceWaitForRefill = false,
            BufferTiming = 0.08,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return BufferableExtends