using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;

namespace Celeste.Mod.LeniencyHelper.UI.Items.SettingHandlers;
class BoolHandler : AbstractHandler<bool>
{
    public override float CalculateMaxWidth(Setting<bool> setting)
        => Math.Max(ActiveFont.Measure(GetText(true)).X, ActiveFont.Measure(GetText(false)).X);

    public override bool Advance(bool value, int direction) => direction > 0;
    public override void CheckBounds(Setting<bool> value, out bool left, out bool right)
    {
        left = value.Player;
        right = !left;
    }

    public override bool CheckValidDir(bool value, Bounds<bool> bounds, int dir)
    {
        return dir > 0 != value;
    }

    public override string GetText(bool value)
    {
        return Dialog.Clean(value ? "OPTIONS_ON" : "OPTIONS_OFF");
    }
}
