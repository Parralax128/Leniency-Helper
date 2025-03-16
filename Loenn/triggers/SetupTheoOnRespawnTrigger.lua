local SetupTheoOnRespawnTrigger = {}

SetupTheoOnRespawnTrigger.name = "LeniencyHelper/SetupTheoOnRespawnTrigger"
SetupTheoOnRespawnTrigger.triggerText = "Setup Theo on respawn Trigger"
SetupTheoOnRespawnTrigger.depth = -1000000
SetupTheoOnRespawnTrigger.nodeLimits = {2, 2}
SetupTheoOnRespawnTrigger.placements = {
    {
        name = "Setup Theo on respawn Trigger",
        data = { 
            SpeedX = 0,
            SpeedY = 0,
            Blacklist = "",
            Whitelist = "",
            VariableName = "",
            Value = false
        }
    }
}

return SetupTheoOnRespawnTrigger