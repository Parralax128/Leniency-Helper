using System;
namespace Celeste.Mod.LeniencyHelper.TweakData.Settings;
public class Bounds<T>
{
    public T Min;
    public T Max;

    public bool Check(T value)
    {
        return (value as IComparable).CompareTo(Min) >= 0 && (value as IComparable).CompareTo(Max) <= 0;
    }
    public Bounds(T min, T max)
    {
        Min = min;
        Max = max;
    }
}