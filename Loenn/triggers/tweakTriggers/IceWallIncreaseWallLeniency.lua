local IceWallIncreaseWallLeniency = {}

IceWallIncreaseWallLeniency.name = "LeniencyHelper/Triggers/IceWallIncreaseWallLeniency"
IceWallIncreaseWallLeniency.triggerText = "Icewall increase Wall-leniency Trigger"
IceWallIncreaseWallLeniency.depth = -1000000
IceWallIncreaseWallLeniency.placements = {
    {
        name = "Icewall increase Wall-leniency Trigger",
        data = { 
            AdditionalLeniency = 3,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return IceWallIncreaseWallLeniency