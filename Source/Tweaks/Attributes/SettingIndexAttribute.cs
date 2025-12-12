using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

[AttributeUsage(AttributeTargets.Field)]
class SettingIndexAttribute : Attribute
{
    public string setting;

    public SettingIndexAttribute(string setting = null)
    {
        this.setting = setting;
    }
}