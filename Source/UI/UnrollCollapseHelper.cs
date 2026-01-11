using Celeste.Mod.LeniencyHelper.UI.Items;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.LeniencyHelper.UI;

class UnrollCollapseHelper
{
    const float e = 2.71828f;

    public enum States
    {
        Collapsing,
        Invisible,
        Unrolling,
        Visible
    }

    public States State = States.Invisible;

    float timer = 0f;
    float CollapseDuration;
    float UnrollDuration;

    float staticHeight;

    Action<bool> VisibilitySetter = null;
    Coroutine Coroutine = null;
    Func<IEnumerator> unrollRoutine = null;
    Func<IEnumerator> collapseRoutine = null;

    public UnrollCollapseHelper(float collapseDuration, float unrollDuration)
    {
        CollapseDuration = collapseDuration;
        UnrollDuration = unrollDuration;
    }
    public UnrollCollapseHelper(float collapseDuration, float unrollDuration, Action<bool> visibilitySetter,
        Coroutine coroutine, Func<IEnumerator> unrollRoutine, Func<IEnumerator> collapseRoutine)
            : this(collapseDuration, unrollDuration)
    {
        Coroutine = coroutine;
        this.unrollRoutine = unrollRoutine;
        this.collapseRoutine = collapseRoutine;

        VisibilitySetter = visibilitySetter;
    }

    public void Collapse()
    {
        if (State == States.Invisible)
        {
            Coroutine?.Cancel();
            return;
        }

        if (State == States.Unrolling) timer = CollapseDuration * (1f - timer / UnrollDuration);
        else timer = 0f;

        if (Coroutine != null) Coroutine.Replace(collapseRoutine.Invoke());
        else State = States.Collapsing;
    }
    public void Unroll()
    {
        if (State == States.Collapsing) timer = UnrollDuration * (1f - timer / CollapseDuration);
        else timer = 0f;

        VisibilitySetter?.Invoke(true);

        if (Coroutine != null) Coroutine.Replace(unrollRoutine.Invoke());
        else State = States.Unrolling;
    }

    public void Update()
    {
        Coroutine?.Update();

        switch (State)
        {
            case States.Unrolling when timer < UnrollDuration:
                timer += Engine.RawDeltaTime;

                if (timer >= UnrollDuration)
                {
                    timer = UnrollDuration;
                    State = States.Visible;
                }
                return;

            case States.Collapsing when timer < CollapseDuration:
                
                timer += Engine.RawDeltaTime;
                if (timer >= CollapseDuration)
                {
                    timer = CollapseDuration;

                    State = States.Invisible;
                    VisibilitySetter?.Invoke(false);
                }
                return;
        }
    }
    public float GetHeight(bool visible, float mult)
    {
        if (State == States.Visible) return staticHeight;
        if (!visible || State == States.Invisible) return 0f;

        float x = State switch
        {
            States.Unrolling => timer / UnrollDuration,
            States.Collapsing => 1f - timer / CollapseDuration,
            _ => throw new ArgumentOutOfRangeException()
        };
        float easedFunction = 1f - (float)Math.Pow(e, -(x*x * e*e));

        return staticHeight = easedFunction * mult;
    }
}