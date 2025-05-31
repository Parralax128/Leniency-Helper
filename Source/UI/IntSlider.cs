using System;
using Microsoft.Xna.Framework;
using static Celeste.TextMenu;
using static Celeste.Mod.LeniencyHelper.SettingMaster;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.UI;

public class IntSlider : Option<int>
{
    public int value = 0;
    private int min, max;
    private float len = 0f;
    public string settingName;
    public IntSlider(string label, int min, int max, int defaultValue, string sessionName)
        : base(label)
    {
        value = defaultValue;
        this.max = max;
        this.min = min;

        settingName = sessionName;

        float maxLen = 0;

        for (float c = min; c <= max; c++)
        {
            float currentLen = ActiveFont.Measure(c.ToString()).X;
            if (maxLen < currentLen) maxLen = currentLen;
        }
        len = maxLen + 120f;
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
    public override float LeftWidth()
    {
        return len;
    }

    public override void Render(Vector2 position, bool highlighted)
    {
        float alpha = Container.Alpha;
        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        Color color = Disabled ? Color.DarkSlateGray : (highlighted ? Container.HighlightColor : UnselectedColor) * alpha;
        ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);

        float num = len;
        ActiveFont.DrawOutline(value.ToString(), position + new Vector2(Container.Width - num * 0.5f + lastDir * ValueWiggler.Value * 8f, 0f),
            new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);

        Vector2 vector = Vector2.UnitX * (highlighted ? (float)Math.Sin(sine * 4f) * 4f : 0f);

        bool flag = value > min;

        Color color2 = flag ? color : Color.DarkSlateGray * alpha;
        Vector2 position2 = position + new Vector2(Container.Width - num + 40f + (lastDir < 0 ?
            (0f - ValueWiggler.Value) * 8f : 0f), 0f) - (flag ? vector : Vector2.Zero);
        ActiveFont.DrawOutline("<", position2, new Vector2(0.5f, 0.5f), Vector2.One, color2, 2f, strokeColor);

        bool flag2 = value < max;

        Color color3 = flag2 ? color : Color.DarkSlateGray * alpha;
        Vector2 position3 = position + new Vector2(Container.Width - 40f + (lastDir > 0 ? ValueWiggler.Value * 8f : 0f), 0f) + (flag2 ? vector : Vector2.Zero);
        ActiveFont.DrawOutline(">", position3, new Vector2(0.5f, 0.5f), Vector2.One, color3, 2f, strokeColor);
    }
}