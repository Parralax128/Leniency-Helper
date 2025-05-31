local trigger = {}

trigger.name = "LeniencyHelper/ConsistentTheoSpinnerBounceTrigger"
trigger.triggerText = "Consistent Theo Spinner-bounce Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Consistent Theo Spinner-bounce Trigger",
        data = {
            BounceDirection = "All",
            ForceLoadSpinners = true,
            Enabled = true,
            RevertOnLeave = true,
            Flag = "",
            OneUse = false            
        }
    }
}

trigger.fieldInformation = {
    BounceDirection = {fieldType="string",options={"None","Left","Right","All"},editable=false}
}

return trigger