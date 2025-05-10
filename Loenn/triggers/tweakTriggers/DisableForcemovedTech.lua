local DisableForcemovedTech = {}

DisableForcemovedTech.name = "LeniencyHelper/Triggers/DisableForcemovedTech"
DisableForcemovedTech.triggerText = "Disable forcemoved tech"
DisableForcemovedTech.depth = -1000000
DisableForcemovedTech.placements = {
    {
        name = "Disable forcemoved tech Trigger",
        data = { 
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return DisableForcemovedTech