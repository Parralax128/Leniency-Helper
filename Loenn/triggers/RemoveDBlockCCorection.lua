local RemoveDBlockCCorection = {}

RemoveDBlockCCorection.name = "LeniencyHelper/Triggers/RemoveDBlockCCorection"
RemoveDBlockCCorection.triggerText = "Remove Dreamblock corner-correction Trigger"
RemoveDBlockCCorection.depth = -1000000
RemoveDBlockCCorection.placements = {
    {
        name = "Remove Dreamblock corner-correction Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return RemoveDBlockCCorection