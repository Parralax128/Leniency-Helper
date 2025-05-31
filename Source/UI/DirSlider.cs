using Celeste.Mod.LeniencyHelper.Tweaks;
using IL.MonoMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.SettingMaster;
using Monocle;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.UI;

public class DirSlider : TextMenu.Option<Dirs>
{
    public Dirs value;
    public string settingName;
    private float len;
    private float CalcLen()
    {
        float maxLen = 0f;
        for (int c = 0; c < 6; c++)
        {
            string dir;
            switch (c)
            {
                case 0: dir = "Up"; break;
                case 1: dir = "Down"; break;
                case 2: dir = "Left"; break;
                case 3: dir = "Right"; break;
                case 4: dir = "All"; break;
                default: dir = "None"; break;
            }
            if (maxLen < ActiveFont.Measure(Dialog.Clean(dir)).X)
            {
                maxLen = ActiveFont.Measure(Dialog.Clean(dir)).X;
            }
        }
        return maxLen + 120f;
    }
    public DirSlider(string label, Dirs defaultValue, string settingName) : base(label)
    {
        this.settingName = settingName;
        value = defaultValue;
        len = CalcLen();
    }

    private Dirs GetDir(bool prev)
    {
        if (prev)
        {
            if (value == Dirs.Up) return Dirs.Up;
            else
            {
                switch (value)
                {
                    case Dirs.Down: return Dirs.Up;
                    case Dirs.Left: return Dirs.Down;
                    case Dirs.Right: return Dirs.Left;
                    case Dirs.All: return Dirs.Right;
                    default: return Dirs.All;
                }
            }
        }
        else
        {
            if (value == Dirs.None) return Dirs.None;
            else
            {
                switch (value)
                {
                    case Dirs.Up: return Dirs.Down;
                    case Dirs.Down: return Dirs.Left;
                    case Dirs.Left: return Dirs.Right;
                    case Dirs.Right: return Dirs.All;
                    default: return Dirs.None;
                }
            }
        }
    }
    public override void LeftPressed()
    {
        Audio.Play("event:/ui/main/button_toggle_off");
        lastDir = -1;
        value = GetDir(true);

        ValueWiggler.Start();

        ChangedValue();
    }
    public override void RightPressed()
    {
        Audio.Play("event:/ui/main/button_toggle_on");
        lastDir = 1;
        value = GetDir(false);

        ValueWiggler.Start();

        ChangedValue();
    }
    public void ChangedValue()
    {
        SetPlayerSetting(settingName, value);
    }
    public override void ConfirmPressed()
    {
        value = GetDefaultSetting<Dirs>(settingName);
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

        float num = RightWidth();
        ActiveFont.DrawOutline(Dialog.Clean(value.ToString()), position + new Vector2(Container.Width - num * 0.5f + lastDir * ValueWiggler.Value * 8f, 0f),
            new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);

        Vector2 vector = Vector2.UnitX * (highlighted ? (float)Math.Sin(sine * 4f) * 4f : 0f);

        bool flag = value != Dirs.Up;

        Color color2 = flag ? color : Color.DarkSlateGray * alpha;
        Vector2 position2 = position + new Vector2(Container.Width - num + 40f + (lastDir < 0 ?
            (0f - ValueWiggler.Value) * 8f : 0f), 0f) - (flag ? vector : Vector2.Zero);
        ActiveFont.DrawOutline("<", position2, new Vector2(0.5f, 0.5f), Vector2.One, color2, 2f, strokeColor);

        bool flag2 = value != Dirs.None;

        Color color3 = flag2 ? color : Color.DarkSlateGray * alpha;
        Vector2 position3 = position + new Vector2(Container.Width - 40f + (lastDir > 0 ? ValueWiggler.Value * 8f : 0f), 0f) + (flag2 ? vector : Vector2.Zero);
        ActiveFont.DrawOutline(">", position3, new Vector2(0.5f, 0.5f), Vector2.One, color3, 2f, strokeColor);
    }

}
