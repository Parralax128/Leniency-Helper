using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class HelperValueWrapper<T>
{
    private Func<object> getter;
    private Action<object> setter;

    public void Set(T val) => setter.Invoke(val);
    public static implicit operator T(HelperValueWrapper<T> wrapper) => (T)wrapper.getter.Invoke();

    public T Default { get; private set; }
    public HelperValueWrapper(T defaultValue)
    {
        Default = defaultValue;
    }
}