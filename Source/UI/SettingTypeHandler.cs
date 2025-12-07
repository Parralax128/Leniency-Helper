using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.GaussianBlur;

namespace Celeste.Mod.LeniencyHelper.UI;
public abstract class SettingTypeHandler<T>
{
    public abstract float CalculateMaxWidth(Setting<T> setting);

    public Action<Setting<T>> OnJournalPressed = null;
    public abstract T Advance(T value, int direction);
    public abstract void CheckBounds(TweakSettings.Setting<T> value, out bool left, out bool right);
    public abstract bool CheckValidDir(T value, Bounds<T> bounds, int dir);
    public abstract string GetText(T value);
}

public class BoolHandler : SettingTypeHandler<bool>
{
    public override float CalculateMaxWidth(Setting<bool> setting)
        => Math.Max(ActiveFont.Measure(GetText(true)).X, ActiveFont.Measure(GetText(false)).X);
    
    public override bool Advance(bool value, int direction) => direction > 0;
    public override void CheckBounds(TweakSettings.Setting<bool> value, out bool left, out bool right)
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

public abstract class ComparableSettingType<T> : SettingTypeHandler<T> where T : IComparable<T>
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

public class IntHandler : ComparableSettingType<int>
{
    public override int Advance(int value, int direction) => value + direction;
    public override string GetText(int value) => value.ToString();
}

public class FloatHandler : ComparableSettingType<float>
{
    public override float Advance(float value, int direction) => value + direction * 0.01f;
    public override string GetText(float value) => Math.Round(value, 2).ToString();
}

public class TimeHandler : ComparableSettingType<Time>
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

public class EnumHandler<EnumT> : SettingTypeHandler<EnumT> where EnumT : struct, Enum
{
    private static EnumT[] values;

    public EnumHandler() => values = Enum.GetValues<EnumT>();

    public override float CalculateMaxWidth(Setting<EnumT> setting)
    {
        float len = 0f;
        foreach (EnumT value in values)
        {
            len = Math.Max(len, ActiveFont.Measure(GetText(value)).X);
        }
        return len;
    }

    public override EnumT Advance(EnumT value, int direction)
        => values[Math.Clamp(Array.IndexOf(values, value) + direction, 0, values.Length - 1)];
    public override void CheckBounds(Setting<EnumT> value, out bool left, out bool right)
    {
        int index = Array.IndexOf(values, value.Player);
        left = index > 0;
        right = index < values.Length - 1;
    }
    public override bool CheckValidDir(EnumT value, Bounds<EnumT> bounds, int dir)
    {
        int targetIndex = Array.IndexOf(values, value) + dir;
        return targetIndex >= 0 && targetIndex <= values.Length - 1;
    }

    public override string GetText(EnumT value)
    {
        return DialogUtils.Enum(value);
    }
}