namespace Celeste.Mod.LeniencyHelper.UI.Items.SettingHandlers;

class IntHandler : ComparableHandler<int>
{
    public override int Advance(int value, int direction) => value + direction;
    public override string GetText(int value) => value.ToString();
}