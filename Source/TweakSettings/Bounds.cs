using System;
namespace Celeste.Mod.LeniencyHelper.TweakSettings;
public class Bounds<T>
{
    private bool hasMin = true;
    public T Min { get; private set; }
    
    private bool hasMax = true;
    public T Max {get; private set; }

    public bool Check(T value)
    {
        return (hasMin ? (value as IComparable<T>).CompareTo(Min) >= 0 : true)
            && (hasMax ? (value as IComparable<T>).CompareTo(Max) <= 0 : true);
    }
    public void Check(T value, out bool withMin, out bool withMax)
    {
        withMin = hasMin ? (value as IComparable<T>).CompareTo(Min) >= 0 : true;
        withMax = hasMax ? (value as IComparable<T>).CompareTo(Max) <= 0 : true;
    }

    public Bounds(T min, T max)
    {
        if (min != null) Min = min;
        else hasMin = false;

        if (max != null) Max = max;
        else hasMax = false;
    }
    public Bounds(T min)
    {
        Min = min;
        hasMin = true;

        hasMax = false;
    }
}