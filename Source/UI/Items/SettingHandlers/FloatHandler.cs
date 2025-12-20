using System;

namespace Celeste.Mod.LeniencyHelper.UI.Items.SettingHandlers;

class FloatHandler : ComparableHandler<float>
{
    public override float Advance(float value, int direction) => value + direction * 0.01f;
    public override string GetText(float value) => Math.Round(value, 2).ToString();
}