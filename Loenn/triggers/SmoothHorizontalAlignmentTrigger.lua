local trigger = {}

trigger.name = "LeniencyHelper/SmoothHorizontalAlignmentTrigger"
trigger.triggerText = "Smooth Horizontal Alignment Trigger"
trigger.depth = -100000
trigger.nodeLimits = {1, 1}
trigger.placements = {
    {
        name = "Smooth Horizontal Alignment Trigger",
        data = {
            Direction = "Downwards",
            Easing = "Sine",
            Flag = "",
            OneUse = false
        }
    }
}

trigger.fieldInformation = {
    Direction = {fieldType="string",options={"Downwards","Upwards"}, editable=false},
    Easing = {fieldType="string",options={"Linear","Sine","Quad","Cube","Quint","Exponent","Back","BigBack","Elastic"}, editable=false}
}

return trigger