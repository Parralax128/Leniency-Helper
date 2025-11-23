using System;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.SettingMaster;
using Monocle;
using static Celeste.Mod.LeniencyHelper.Extensions;

namespace Celeste.Mod.LeniencyHelper.UI;

public class FloatSlider : TweakSetting<float>
{
    public bool isTimer = false;

    private float min, max;
    private int framesMin, framesMax;
    private int digits;

    public bool transitionIntoFrames = false;

    private float rightLen = 0f;
    public FloatSlider(float min, float max, int digits,
        string tweak, string settingName, TextMenu addedTo) : base(tweak, settingName, addedTo, true)
    {
        value = (float)Math.Round(GetSetting<float>(settingName, tweak), digits);

        this.max = (float)Math.Round(max, digits);
        if (this.max > max) this.max = (float)Math.Round(max - (float)Math.Pow(10, -digits), digits);

        this.min = min;
        framesMax = (int)Math.Floor(max * Engine.FPS);
        framesMin = (int)Math.Floor(min * Engine.FPS);
        this.digits = digits;

        float maxLen = 0;
        for (float c = min; c < max; c = (float)Math.Round(Calc.Approach(c, max, (float)Math.Pow(10, -digits)), digits))
        {
            float currentLen = ActiveFont.Measure(c.ToString()).X;
            if (maxLen < currentLen)
            {
                maxLen = currentLen;
            }
        }
        rightLen = maxLen;
    }
    public void SwapMode(bool toFrames)
    {
        if (toFrames == transitionIntoFrames) return;

        if (toFrames)
        {
            value = (float)Math.Floor(value * Engine.FPS);
        }
        else value = (float)Math.Round(value / Engine.FPS, digits);

        transitionIntoFrames = toFrames;
    }
    public override void LeftPressed()
    {
        Audio.Play("event:/ui/main/button_toggle_off");
        lastDir = -1;

        if (transitionIntoFrames) value = (float)Math.Floor(Calc.Approach(value, framesMin, 1));
        else value = (float)Math.Round(Calc.Approach(value, min, (float)Math.Pow(10, -digits)), digits);

        ValueWiggler.Start();

        ChangedValue();
    }
    public override void RightPressed()
    {
        Audio.Play("event:/ui/main/button_toggle_on");
        lastDir = 1;

        if (transitionIntoFrames) value = (float)Math.Floor(Calc.Approach(value, framesMax, 1));
        else value = (float)Math.Round(Calc.Approach(value, max, (float)Math.Pow(10, -digits)), digits);

        ValueWiggler.Start();

        ChangedValue();
    }
    public void ChangedValue()
    {
        SetPlayerSetting(settingName, value);
        if (OnValueChange != null) OnValueChange(value);
    }

    public override void ConfirmPressed()
    {
        value = transitionIntoFrames ? (float)Math.Floor(GetDefaultSetting<float>(settingName) * Engine.FPS) : GetDefaultSetting<float>(settingName);
        ChangedValue();
    }
    public override float RightWidth() => rightLen;

    public override void Render(Vector2 position, bool selected)
    {
        BeforeRender(ref position, selected);


        DrawLabel();

        DrawRightText(value.ToString(), scale: 0.8f);

        Vector2 sineShift = Vector2.UnitX * (selected ? (float)Math.Sin(sine * 4f) * 4f : 0f);

        bool notMinimal = transitionIntoFrames ? value > framesMin : value > min;
        DrawRightText("<", -40f*Engine.ScreenMatrix.M11 - (notMinimal ? sineShift : Vector2.Zero).X, inactiveColor: !notMinimal);

        bool notMaximal = transitionIntoFrames ? value < framesMax: value < max;
        DrawRightText(">", 40f*Engine.ScreenMatrix.M11 + (notMaximal ? sineShift : Vector2.Zero).X, inactiveColor: !notMaximal);
    }
}