using System;
using Monocle;

namespace Celeste.Mod.LeniencyHelper;

public class Timer
{
    public enum Type { Gameplay, Input }

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
    public Timer(float time, Type type = Type.Gameplay, Action onComplete = null) : this(type)
    {
        startTime = counter = time;
        OnComplete = onComplete;
    }
    public Timer(Type type = Type.Gameplay) 
    {
        Expired = true;
        TimerManager.Add(this, type); 
    }

    public static implicit operator float(Timer createFrom) => createFrom.counter;
    public static implicit operator bool(Timer timer) => !timer.Expired;
}