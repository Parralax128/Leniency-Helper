using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Tweaks;
using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

public  class DelayedClimbtriggerComponent : Component
{
    private static float Delay => DelayedClimbtrigger.GetSetting<Time>("MaxDelay");
    public float climbtriggerTimer;
    private Solid Solid => (Solid) Entity;
    public DelayedClimbtriggerComponent() : base(true, false) { }

    public override void Update()
    {
        base.Update();
        if (climbtriggerTimer > 0f) climbtriggerTimer -= Engine.DeltaTime;

        DelayedClimbtrigger.useOrigCheck = true;

        if (Scene.Tracker.GetEntity<Player>() is Player player && player.IsRiding(Solid))
        {
            climbtriggerTimer = Delay;
        }
        
        DelayedClimbtrigger.useOrigCheck = false;
    }
}
