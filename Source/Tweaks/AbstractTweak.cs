using System.Runtime.CompilerServices;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class AbstractTweak<TweakType>   where TweakType : AbstractTweak<TweakType>
{
    static readonly Tweak tweak = System.Enum.Parse<Tweak>(typeof(TweakType).Name);

    
    public static bool Enabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TweakData.Tweaks[tweak].Enabled;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetSetting<T>(int index = 0) => TweakData.Tweaks[tweak].GetSetting<T>(index);
    public static int GetFlexDistance(int index, float speed) =>
        FlexDistance.Get(GetSetting<FlexDistance.Modes>(index),
        GetSetting<int>(index+1), GetSetting<Time>(index+2), speed);

    public static int GetFlexDistance(float speed) => 
        FlexDistance.Get(GetSetting<FlexDistance.Modes>(0), GetSetting<int>(1), GetSetting<Time>(2), speed);
}