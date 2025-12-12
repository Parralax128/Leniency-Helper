using Celeste.Mod.LeniencyHelper.TweakSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.TrackSpinner;

namespace Celeste.Mod.LeniencyHelper;
class FlexDistance
{
    public enum Modes
    { 
        Static,
        Dynamic
    }

    public int StaticValue;

    public Modes Mode;
    public Time Time;
    public int Get(float Speed)
        => Mode == Modes.Static ? StaticValue : (int)(Time.Value * Speed);


    public FlexDistance(int staticValue, Time timer, Modes mode)
    {
        StaticValue = staticValue;
        Time = timer;
        this.Mode = mode;
    }
}