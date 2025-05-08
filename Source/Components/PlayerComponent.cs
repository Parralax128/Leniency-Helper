using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

public class PlayerComponent : Component
{
    private string tweakName;
    public PlayerComponent(string tweakName) : base(true, true)
    {
        if (Entity is not Player) RemoveSelf();
        this.tweakName = tweakName;
    }
    public override void Update()
    {
        base.Update();
        Visible = SettingMaster.GetTweakEnabled(tweakName);
    }
}