local DashbounceTrigger = {}

DashbounceTrigger.name = "LeniencyHelper/Triggers/CustomDashbounceTiming"
DashbounceTrigger.triggerText = "Custom Dashbounce Timing"
DashbounceTrigger.depth = -1000000
DashbounceTrigger.placements = {
    {
        name = "Custom Dashbounce Timing Trigger",
        data = {
            Timing = 0.05,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return DashbounceTrigger