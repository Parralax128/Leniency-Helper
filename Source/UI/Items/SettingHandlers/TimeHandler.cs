namespace Celeste.Mod.LeniencyHelper.UI.Items.SettingHandlers;

class TimeHandler : ComparableHandler<Time>
{
    public TimeHandler()
    {
        OnJournalPressed = (time) => time.Player.SwapMode();
    }

    public override Time Advance(Time value, int direction)
    {
        if (value.Mode == Time.Modes.Frames) return value + direction;
        return value + direction * 0.01f;
    }
    public override string GetText(Time value) => value.ToString();
}