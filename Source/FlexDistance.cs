using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.TrackSpinner;

namespace Celeste.Mod.LeniencyHelper;
public class FlexDistance
{
    public int StaticValue;

    public bool Dynamic = false;
    public Time Timer;
    public int Get(SettingSource source, float Speed)
        =>  Dynamic ? StaticValue : (int)(Timer.Value * Speed);


    public FlexDistance(int staticValue, bool dynamic, Time timer)
    {
        StaticValue = staticValue;
        Dynamic = dynamic;
        Timer = timer;
    }
}