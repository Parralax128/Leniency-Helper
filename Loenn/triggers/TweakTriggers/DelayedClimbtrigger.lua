local DelayedClimbtrigger = {}

DelayedClimbtrigger.name = "LeniencyHelper/Triggers/DelayedClimbtrigger"
DelayedClimbtrigger.triggerText = "Delayed Climbtrigger Trigger"
DelayedClimbtrigger.depth = -1000000
DelayedClimbtrigger.placements = {
    {
        name = "Delayed Climbtrigger Trigger",
        data = {
            TriggerDelay = 0.25,
            CountTimeInFrames = false,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return DelayedClimbtrigger