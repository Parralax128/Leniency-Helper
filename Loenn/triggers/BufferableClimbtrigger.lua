local BufferableClimbtrigger = {}

BufferableClimbtrigger.name = "LeniencyHelper/Triggers/BufferableClimbtrigger"
BufferableClimbtrigger.triggerText = "Bufferable Climbtrigger Trigger"
BufferableClimbtrigger.depth = -1000000
BufferableClimbtrigger.placements = {
    {
        name = "Bufferable Climbtrigger Trigger",
        data = {
            InstantUpwardClimbActivation = true,
            ClimbTriggerOnDash = true,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false
        }
    }
}

return BufferableClimbtrigger