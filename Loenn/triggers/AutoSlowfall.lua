local AutoSlowfall = {}

AutoSlowfall.name = "LeniencyHelper/Triggers/AutoSlowfall"
AutoSlowfall.triggerText = "Auto Slowfall Trigger"
AutoSlowfall.depth = -1000000
AutoSlowfall.placements = {
    {
        name = "Auto Slowfall Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false,
        }
    }
}

return AutoSlowfall