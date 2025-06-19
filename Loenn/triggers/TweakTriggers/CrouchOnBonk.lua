local AutoSlowfall = {}

AutoSlowfall.name = "LeniencyHelper/Triggers/CrouchOnBonk"
AutoSlowfall.triggerText = "Crouch on Bonk Trigger"
AutoSlowfall.depth = -1000000
AutoSlowfall.placements = {
    {
        name = "Crouch on Bonk Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false,
        }
    }
}

return AutoSlowfall