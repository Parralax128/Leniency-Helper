using Celeste.Mod.LeniencyHelper.Module;
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

    static AbstractTweak()
    {
        IEnumerable<FieldInfo> targetFields = typeof(TweakType)
            .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(HelperValueWrapper<>));

        if (targetFields == null || targetFields.Count() == 0) return;

        Attributes.TempContainerEntity.Pending.Add(tweak, new());

        int indexCounter = 0;
        foreach (FieldInfo field in targetFields)
        {
            Func<object> getter = () => Attributes.TempContainerEntity.Instance.Get(tweak, indexCounter);
            Action<object> setter = (value) => Attributes.TempContainerEntity.Instance.Set(tweak, indexCounter, value);


            field.FieldType.GetField("getter", BindingFlags.NonPublic).SetValue(field.GetValue(null), getter);
            field.FieldType.GetField("setter", BindingFlags.NonPublic).SetValue(field.GetValue(null), setter);


            Attributes.TempContainerEntity.Pending[tweak].Add(field.FieldType.GetMethod("get_Default").Invoke(field.GetValue(null), null));

            indexCounter++;
        }
    }
}