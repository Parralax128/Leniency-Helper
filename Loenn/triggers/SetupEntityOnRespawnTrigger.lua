local trigger = {}

trigger.name = "LeniencyHelper/SetupEntityOnRespawnTrigger"
trigger.triggerText = "Setup Entity on respawn Trigger"
trigger.depth = -1000000
trigger.nodeLimits = {2, 2}
trigger.placements = {
    {
        name = "Setup Entity on respawn Trigger",
        data = { 
            SpeedX = 0,
            SpeedY = 0,
            Blacklist = "",
            Whitelist = "",
        }
    }
}

return trigger