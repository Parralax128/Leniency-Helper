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
    public Time(int frames) : this((float)frames, Modes.Frames) { }

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
        return other.Value > Value ? -1 : Math.Abs(Value - other.Value) < 0.01f ? 0 : 1;
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


    public static implicit operator Time(float time)
    {
        return new Time(time, Modes.Seconds);
    }
    public static implicit operator Time(int frames)
    {
        return new Time(frames, Modes.Frames);
    }

    public static Time operator +(Time left, Time right)
        => new Time(left.Value + right.Value, left.Mode == Modes.Seconds ? Modes.Seconds : right.Mode);

    public static Time operator +(Time timer, float seconds)
       => new Time(timer.Value + seconds);
    public static Time operator +(Time timer, int frames)
        => new Time(timer.Get(Modes.Frames) + frames, Modes.Frames);

    public override string ToString()
    {
        return Mode == Modes.Seconds ? Value.ToString("F2")+'s' : ((int)Get(Modes.Frames)).ToString()+'f';
    }
}