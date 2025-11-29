using System;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.SettingMaster;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.UI.CustomOptions;

public class DirSlider : TweakSetting<Dirs>
{
    private float? len = null;

    public DirSlider(Tweak tweak, string settingName) : base(tweak, settingName) { }

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
        return len.HasValue ? len.Value : (len = CalcLen()).Value;
    }

    public override void Render(Vector2 position, bool selected)
    {
        BaseRender(value.ToString(), position, selected, value != Dirs.Up, value != Dirs.None);
    }
}