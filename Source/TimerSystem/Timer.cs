using System;
using Monocle;

namespace Celeste.Mod.LeniencyHelper;

public class Timer
{
    float startTime;
    float counter = 0f;
    public bool Expired { get; set; }
    Action OnComplete = null;

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

        TimerManager.Add(this, tag);
    }
    public Timer(int tag = 0) { TimerManager.Add(this, tag); }

    public static implicit operator Timer(Time createFrom) => new Timer(createFrom);
    public static implicit operator float(Timer createFrom) => createFrom.counter;
    public static implicit operator bool(Timer timer) => !timer.Expired;
}