using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.LeniencyHelper.TweakData.Settings;

public abstract class CompositeSetting<T> : SettingContainer
{
    public string Name;
    public abstract T Get(SettingSource source);
}