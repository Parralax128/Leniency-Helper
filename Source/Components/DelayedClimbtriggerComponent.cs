using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.LeniencyHelper.Tweaks;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

public  class DelayedClimbtriggerComponent : Component
{
    private static float Delay => SettingMaster.GetTime("triggerDelay", "DelayedClimbtrigger") /*SettingMaster.GetSetting<float>("climbtriggerDelay", "DelayedClimbtrigger")*/;
    public float climbtriggerTimer;
    private Solid Solid => (Solid) Entity;
    public DelayedClimbtriggerComponent() : base(true, false) { }

    public override void Update()
    {
        base.Update();
        if (climbtriggerTimer > 0f) climbtriggerTimer -= Engine.DeltaTime;

        DelayedClimbtrigger.useOrigCheck = true;
        if (LeniencyHelperModule.player.IsRiding(Solid))
        {
            climbtriggerTimer = Delay;
            Logger.Info("", $"triggered timer! {LeniencyHelperModule.player.Position}");
        }
        DelayedClimbtrigger.useOrigCheck = false;
    }
}
