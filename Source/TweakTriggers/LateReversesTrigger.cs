﻿using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/LateReverses")]
public class LateReversesTrigger : GenericTweakTrigger
{
    public LateReversesTrigger(EntityData data, Vector2 offset) : base(data, offset, "LateReverses")
    {

    }
}