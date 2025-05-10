local trigger = {}

trigger.name = "LeniencyHelper/Triggers/SolidBlockboostProtection"
trigger.triggerText = "Solid blockboost protection trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Solid blockboost protection trigger",
        data = {
            ProtectionTime = 0.1,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return trigger