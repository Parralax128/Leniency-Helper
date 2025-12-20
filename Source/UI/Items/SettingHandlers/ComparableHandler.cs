using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;

namespace Celeste.Mod.LeniencyHelper.UI.Items.SettingHandlers;

abstract class ComparableHandler<T> : AbstractHandler<T> where T : IComparable<T>
{
    public override float CalculateMaxWidth(Setting<T> setting)
    {
        Bounds<T> bounds = setting.ValueBounds;
        float len = 0f;

        for (T c = bounds.Min; c.CompareTo(bounds.Max) <= 0; c = Advance(c, 1))
        {
            len = Math.Max(len, ActiveFont.Measure(GetText(c)).X);
        }
        return len;
    }
    public override void CheckBounds(Setting<T> value, out bool left, out bool right)
    {
        value.ValueBounds.Check(value.Player, out left, out right);
    }
    public override bool CheckValidDir(T value, Bounds<T> bounds, int dir)
    {
        return bounds.Check(Advance(value, dir));
    }
}
