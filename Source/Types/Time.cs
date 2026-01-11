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

    public float Seconds;

    public Time(float time, Modes mode = Modes.Seconds)
    {
        Seconds = mode == Modes.Frames ? (float)time / FPS : time;       
        Mode = mode;
    }

    public int Frames => (int)(Seconds * FPS);
    public void SwapMode() => Mode = (Modes)(1 - (int)Mode);

    int IComparable<Time>.CompareTo(Time other)
    {
        if (Math.Abs(Seconds - other.Seconds) < 0.0001f) return 0;
        return Seconds < other.Seconds ? -1 : 1;
    }

    object ICloneable.Clone()
    {
        return new Time(Mode == Modes.Frames ? Frames : Seconds, Mode);
    }
    public static bool operator >=(Time left, Time right)
    {
        return left.Seconds >= right.Seconds;
    }
    public static bool operator <=(Time left, Time right)
    {
        return left.Seconds <= right.Seconds;
    }


    public static implicit operator int(Time time)
    {
        return (int)(time.Seconds * FPS);
    }
    public static implicit operator float(Time time)
    {
        return time.Seconds;
    }


    public static implicit operator Time(float time) => new Time(time, Modes.Seconds);
    public static implicit operator Time(int frames) => new Time(frames, Modes.Frames);

    public static Time operator +(Time left, Time right)
        => new Time(left.Seconds + right.Seconds, left.Mode == Modes.Seconds ? Modes.Seconds : right.Mode);

    public static Time operator +(Time time, float seconds) => new Time(time.Seconds + seconds);
    public static Time operator +(Time time, int frames) => new Time(time.Frames + frames, Modes.Frames);

    public override string ToString() => ToString(Mode);
    public string ToString(Modes mode)
    {
        return mode == Modes.Seconds
            ? MathF.Round(Seconds, 2).ToString() + 's'
            : Frames.ToString() + 'f';
    }
}