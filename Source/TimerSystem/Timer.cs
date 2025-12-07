using System;
using Celeste.Mod.LeniencyHelper;
using Monocle;
using MonoMod;

namespace Celeste.Mod.LeniencyHelper;

public class Timer
{
    private float startTime;
    private float counter = 0f;
    public bool Expired { get; private set; }
    private Action OnComplete = null;

    public void Tick()
    {
        if (Expired) return;

        counter -= Engine.DeltaTime;
        if (counter <= 0f)
        {
            Expired = true;
            OnComplete?.Invoke();
        }
    }

    public void Set(float time) => startTime = time;
    public void Launch()
    {
        counter = startTime;
        Expired = false;
    }
    public void Launch(float time)
    {
        counter = startTime = time;
        Expired = false;
    }
    public void Abort()
    {
        counter = 0f;
        Expired = true;
    }

    public Timer(float time, int tag = 0, Action onComplete = null)
    {
        startTime = counter = time;
        Expired = true;
        this.OnComplete = onComplete;

        Debug.Log($"this is not null: {this != null}");
        TimerManager.Add(this, tag);
    }

    public static implicit operator Timer(Time createFrom) => new Timer(createFrom);
    public static implicit operator float(Timer createFrom) => createFrom.counter;
    public static implicit operator Time(Timer createFrom) => new Time(createFrom);
    public static implicit operator bool(Timer timer) => !timer.Expired;
}