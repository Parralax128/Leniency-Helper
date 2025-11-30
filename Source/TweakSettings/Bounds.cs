using System;
namespace Celeste.Mod.LeniencyHelper.TweakSettings;
public class Bounds<T>
{
    private bool hasMin = true;
    private T Min;
    
    private bool hasMax = true;
    private T Max;

    public bool Check(T value)
    {
        return (hasMin ? (value as IComparable).CompareTo(Min) >= 0 : true)
            && (hasMax ? (value as IComparable).CompareTo(Max) <= 0 : true);
    }
    public void Check(T value, out bool withMin, out bool withMax)
    {
        withMin = hasMin ? (value as IComparable).CompareTo(Min) >= 0 : true;
        withMax = hasMax ? (value as IComparable).CompareTo(Max) <= 0 : true;
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