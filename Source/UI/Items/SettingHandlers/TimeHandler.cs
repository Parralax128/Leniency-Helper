using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;

namespace Celeste.Mod.LeniencyHelper.UI.Items.SettingHandlers;

class TimeHandler : ComparableHandler<Time>
{
    public TimeHandler()
    {
        OnJournalPressed = (time) => time.Player.SwapMode();
    }

    public override float CalculateMaxWidth(Setting<Time> setting)
    {
        Bounds<Time> bounds = setting.ValueBounds;
        float len = 0f;

        for (Time c = bounds.Min; c <= bounds.Max; c = Advance(c, 1))
        {
            len = Math.Max(len, 
                Math.Max(ActiveFont.Measure(c.ToString(Time.Modes.Seconds)).X,
                ActiveFont.Measure(c.ToString(Time.Modes.Frames)).X));
        }

        return len;
    }
    public override Time Advance(Time value, int direction)
    {
        if (value.Mode == Time.Modes.Frames) return value + direction;
        return value + direction * 0.01f;
    }
    public override string GetText(Time value) => value.ToString();
}