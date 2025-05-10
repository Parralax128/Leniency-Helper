local NoFailedTech = {}

NoFailedTech.name = "LeniencyHelper/Triggers/NoFailedTech"
NoFailedTech.triggerText = "No Failed Tech Trigger"
NoFailedTech.depth = -1000000
NoFailedTech.placements = {
    {
        name = "No Failed Tech Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return NoFailedTech