local trigger = {}

trigger.name = "LeniencyHelper/InputRequiresBlockboostTrigger"
trigger.triggerText = "Input requires Blockboost Trigger"
trigger.depth = -1000000
trigger.placements = {
    {
        name = "Input requires Blockboost Trigger",
        data = {
            Input = "Jump",
            BlockboostValue = 250.0,
            Vertical = false,
            Mode = "MoreThanOrEqual",
            Flag = "",
            OneUse = false
        }
    }
}

trigger.fieldInformation = {
    Input = {fieldType="string",options={"Jump","Dash","Demo"},editable=false},
    Mode = {fieldType="string",options={"MoreThan","MoreThanOrEqual","LessThan","LessThanOrEqual","IsEqual"},editable=false}
}

return trigger