using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;

namespace Celeste.Mod.LeniencyHelper.UI.Items.SettingHandlers;
abstract class AbstractHandler<T>
{
    public abstract float CalculateMaxWidth(Setting<T> setting);

    public Action<Setting<T>> OnJournalPressed = null;
    public abstract T Advance(T value, int direction);
    public abstract void CheckBounds(Setting<T> value, out bool left, out bool right);
    public abstract bool CheckValidDir(T value, Bounds<T> bounds, int dir);
    public abstract string GetText(T value);
}