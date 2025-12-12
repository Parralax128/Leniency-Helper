using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

class PlayerComponent : Component
{
    Tweak tweak;
    public PlayerComponent(Tweak tweak) : base(true, true)
    {
        if (Entity is not Player) RemoveSelf();
        this.tweak = tweak;
    }
    public override void Update()
    {
        base.Update();
        Visible = tweak.Enabled();
    }
}