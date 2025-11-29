using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.TrackSpinner;

namespace Celeste.Mod.LeniencyHelper.TweakData.Settings;
public class FlexDistance
{
    public int StaticValue;

    public bool Dynamic = false;
    public Timer Timer;
    public int Get(SettingSource source, float Speed)
        =>  Dynamic ? StaticValue : (int)(Timer.Time * Speed);


    public FlexDistance(int staticValue, bool dynamic, Timer timer)
    {
        StaticValue = staticValue;
        Dynamic = dynamic;
        Timer = timer;
    }
}

public class FlexDistanceSetting : CompositeSetting<FlexDistance>
{
    public FlexDistanceSetting(string name)
    {
        this.Name = name;
        Add(new Setting<int>("StaticDistance", 3));
        Add(new Setting<bool>("Dynamic", false));
        Add(new Setting<Timer>("Timer", new Timer(0.05f)));
    }
    public override FlexDistance Get(SettingSource source)
    {
        return new FlexDistance(
            Get<int>("StaticDistance", source),
            Get<bool>("Dynamic", source),
            Get<Timer>("Timer", source));
    }
}