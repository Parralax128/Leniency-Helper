using Celeste.Mod.LeniencyHelper.Module;
using Celeste.Mod.MaxHelpingHand.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using YamlDotNet.Core.Tokens;

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
    public static int GetFlexDistance(int settingIndex, float speed) =>
        FlexDistance.Get(GetSetting<FlexDistance.Modes>(settingIndex), GetSetting<int>(settingIndex+1), GetSetting<Time>(settingIndex+2), speed);
}