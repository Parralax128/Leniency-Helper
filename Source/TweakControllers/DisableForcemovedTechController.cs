﻿using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/DisableForcemovedTech")]
public class DisableForcemovedTechController : GenericTweakController
{
    public DisableForcemovedTechController(EntityData data, Vector2 offset) : base(data, offset, "DisableForcemovedTech")
    {

    }
}