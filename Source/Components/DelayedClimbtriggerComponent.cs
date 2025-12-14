using Celeste.Mod.LeniencyHelper.Tweaks;

namespace Celeste.Mod.LeniencyHelper.Components;

class DelayedClimbtriggerComponent : TweakComponent<Solid, DelayedClimbtrigger>
{
    public Timer ClimbtriggerTimer = new();

    public override void Update()
    {
        base.Update();

        DelayedClimbtrigger.useOrigCheck = true;
        if (Entity.HasPlayerRider())
        {
            ClimbtriggerTimer.Launch(GetSetting<Time>());
        }
        DelayedClimbtrigger.useOrigCheck = false;
    }
}