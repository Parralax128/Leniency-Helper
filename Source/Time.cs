using System;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.LeniencyHelper;

public class Time : IComparable<Time>, ICloneable
{
    public enum Modes
    { 
        Seconds = 0,
        Frames = 1
    }
    const float FPS = 60f;

    public Modes Mode = Modes.Seconds;

    public float Value;

    public Time(float time, Modes mode = Modes.Seconds)
    {
        Value = mode == Modes.Frames ? (float)time / FPS : time;       
        Mode = mode;
    }

    public int Frames => (int)(Value * FPS);
    public void SwapMode() => Mode = (Modes)(1 - (int)Mode);

    int IComparable<Time>.CompareTo(Time other)
    {
        if (Math.Abs(Value - other.Value) < 0.0001f) return 0;
        return Value < other.Value ? -1 : 1;
    }

    object ICloneable.Clone()
    {
        return new Time(Mode == Modes.Frames ? Frames : Value, Mode);
    }

    public Timer CreateTimer(int tag = 0) => new Timer(Value, tag);

    public static bool operator >=(Time left, Time right)
    {
        return left.Value >= right.Value;
    }
    public static bool operator <=(Time left, Time right)
    {
        return left.Value <= right.Value;
    }


    public static implicit operator int(Time time)
    {
        return (int)(time.Value * FPS);
    }
    public static implicit operator float(Time time)
    {
        return time.Value;
    }


    public static implicit operator Time(float time) => new Time(time, Modes.Seconds);
    public static implicit operator Time(int frames) => new Time(frames, Modes.Frames);

    public static Time operator +(Time left, Time right)
        => new Time(left.Value + right.Value, left.Mode == Modes.Seconds ? Modes.Seconds : right.Mode);

    public static Time operator +(Time time, float seconds) => new Time(time.Value + seconds);
    public static Time operator +(Time time, int frames) => new Time(time.Frames + frames, Modes.Frames);

    public override string ToString()
    {
        return Mode == Modes.Seconds
            ? MathF.Round(Math.Abs(Value), 2).ToString()+'s'
            : Frames.ToString()+'f';
    }
}