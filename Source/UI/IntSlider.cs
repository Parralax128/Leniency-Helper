using System;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.SettingMaster;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.UI;

public class IntSlider : TweakSetting<int>
{
    private int min, max;
    private float len = 0f;
    public IntSlider(int min, int max, string tweak, string settingName, TextMenu addedTo)
        : base(tweak, settingName, addedTo)
    {
        this.max = max;
        this.min = min;

        float maxLen = 0;

        for (float c = min; c <= max; c++)
        {
            float currentLen = ActiveFont.Measure(c.ToString()).X;
            if (maxLen < currentLen) maxLen = currentLen;
        }
        len = maxLen;
    }
    public override void LeftPressed()
    {
        Audio.Play("event:/ui/main/button_toggle_off");
        lastDir = -1;
        value = (int)Calc.Approach(value, min, 1);

        ValueWiggler.Start();

        ChangedValue();
    }
    public override void RightPressed()
    {
        Audio.Play("event:/ui/main/button_toggle_on");
        lastDir = 1;
        value = (int)Calc.Approach(value, max, 1);

        ValueWiggler.Start();

        ChangedValue();
    }
    public void ChangedValue()
    {
        SetPlayerSetting(settingName, value);
    }
    public override void ConfirmPressed()
    {
        value = GetDefaultSetting<int>(settingName);
        ChangedValue();
    }
    public override float RightWidth()
    {
        return len;
    }

    public override void Render(Vector2 position, bool selected)
    {
        BeforeRender(ref position, selected);

        DrawLabel();

        DrawRightText(value.ToString(), scale: 0.8f);

        Vector2 sineShift = Vector2.UnitX * (selected ? (float)Math.Sin(sine * 4f) * 4f : 0f);

        bool notMinimal = value > min;
        DrawRightText("<", -40f * Engine.ScreenMatrix.M11 - (notMinimal ? sineShift : Vector2.Zero).X, inactiveColor: !notMinimal);

        bool notMaximal = value < max;
        DrawRightText(">", 40f * Engine.ScreenMatrix.M11 + (notMaximal ? sineShift : Vector2.Zero).X, inactiveColor: !notMaximal);
    }
}