using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;

namespace Celeste.Mod.LeniencyHelper.UI.Items.SettingHandlers;

class EnumHandler<EnumT> : AbstractHandler<EnumT> where EnumT : struct, Enum
{
    static EnumT[] Values = Enum.GetValues<EnumT>();

    public override float CalculateMaxWidth(Setting<EnumT> setting)
    {
        float len = 0f;
        foreach (EnumT value in Values)
        {
            len = Math.Max(len, ActiveFont.Measure(GetText(value)).X);
        }
        return len;
    }

    public override EnumT Advance(EnumT value, int direction)
        => Values[Math.Clamp(Array.IndexOf(Values, value) + direction, 0, Values.Length - 1)];
    public override void CheckBounds(Setting<EnumT> value, out bool left, out bool right)
    {
        int index = Array.IndexOf(Values, value.Player);
        left = index > 0;
        right = index < Values.Length - 1;
    }
    // speical overload for the TweakSlider ._.
    public void CheckBounds(EnumT value, out bool left, out bool right)
    {
        int index = Array.IndexOf(Values, value);
        left = index > 0;
        right = index < Values.Length - 1;
    }

    public override bool CheckValidDir(EnumT value, Bounds<EnumT> bounds, int dir)
    {
        int targetIndex = Array.IndexOf(Values, value) + dir;
        return targetIndex >= 0 && targetIndex <= Values.Length - 1;
    }

    public override string GetText(EnumT value)
    {
        return DialogUtils.Enum(value, DialogUtils.Precision.Localized);
    }
}