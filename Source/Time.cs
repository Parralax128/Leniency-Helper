using System;

namespace Celeste.Mod.LeniencyHelper;

public class Time : IComparable<Time>, ICloneable
{
    public enum Modes
    { 
        Seconds = 0,
        Frames = 1
    }
    private const float FPS = 60f;

    public Modes Mode = Modes.Seconds;

    public float Value;

    public Time(float time, Modes mode = Modes.Seconds)
    {
        Value = mode == Modes.Frames ? time / FPS : time;
        Mode = mode;
    }
    public void Set(float time, Modes mode = Modes.Seconds)
    {
        Value = mode == Modes.Frames ? time / FPS : time;
    }
    public float Get(Modes mode = Modes.Seconds)
    {
        return mode == Modes.Frames ? Value * FPS : Value;
    }

    public void SwapMode() => Mode = (Modes)(1 - (int)Mode);

    int IComparable<Time>.CompareTo(Time other)
    {
        return other.Value > Value ? 1 : other.Value == Value ? 0 : -1;
    }

    object ICloneable.Clone()
    {
        return new Time(Value, Mode);
    }

    public static bool operator >=(Time left, Time right)
    {
        return left.Value >= right.Value;
    }
    public static bool operator <=(Time left, Time right)
    {
        return left.Value <= right.Value;
    }


    public static implicit operator int(Time timer)
    {
        return (int)(timer.Value * FPS);
    }
    public static implicit operator float(Time timer)
    {
        return timer.Mode == Modes.Frames ? timer.Value * FPS : timer.Value;
    }


    public static explicit operator Time(float time)
    {
        return new Time(time, Modes.Seconds);
    }
    public static explicit operator Time(int frames)
    {
        return new Time(frames, Modes.Frames);
    }

    public override string ToString()
    {
        return Mode == Modes.Seconds ? Value.ToString() : $"{Get(Modes.Frames)}F";
    }
}