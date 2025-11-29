using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.LeniencyHelper.TweakData.Settings;

public struct Timer : IComparable
{
    public enum Modes
    { 
        Seconds,
        Frames
    }
    private const float FPS = 60f;

    public Modes Mode = Modes.Seconds;

    public float Time;

    public Timer(float time, Modes mode = Modes.Seconds)
    {
        Time = mode == Modes.Frames ? time / FPS : time;
        Mode = mode;
    }
    public void Set(float time, Modes mode = Modes.Seconds)
    {
        Time = mode == Modes.Frames ? time / FPS : time;
    }
    public float Get(Modes mode = Modes.Seconds)
    {
        return mode == Modes.Frames ? Time * FPS : Time;
    }

    int IComparable.CompareTo(object other)
    {
        if (other is Timer timer)
        {
            return timer.Time > Time ? 1 : timer.Time == Time ? 0 : -1;
        }
        return 0;
    }

    public static bool operator >=(Timer left, Timer right)
    {
        return left.Time >= right.Time;
    }
    public static bool operator <=(Timer left, Timer right)
    {
        return left.Time <= right.Time;
    }


    public static implicit operator int(Timer timer)
    {
        return (int)(timer.Time * FPS);
    }
    public static implicit operator float(Timer timer)
    {
        return timer.Mode == Modes.Frames ? timer.Time * FPS : timer.Time;
    }


    public static explicit operator Timer(float time)
    {
        return new Timer(time, Modes.Seconds);
    }
    public static explicit operator Timer(int frames)
    {
        return new Timer(frames, Modes.Frames);
    }
}