using System;
namespace Celeste.Mod.LeniencyHelper.TweakSettings;
class Bounds<T>
{
    public T Min { get; private set; }
    public T Max { get; private set; }

    public bool Check(T value)
    {
        return (value as IComparable<T>).CompareTo(Min) >= 0
            && (value as IComparable<T>).CompareTo(Max) <= 0;
    }
    public void Check(T value, out bool left, out bool right)
    {
        left = (value as IComparable<T>).CompareTo(Min) > 0;
        right = (value as IComparable<T>).CompareTo(Max) < 0;
    }

    public Bounds(T min, T max)
    {
        Min = min;
        Max = max;
    }
}