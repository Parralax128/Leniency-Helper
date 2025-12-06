using System;
using Celeste.Mod.LeniencyHelper;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper;
struct Timer
{
    private float counter = 0f;
    public bool Expired { get; private set; } = false;
    private Action OnComplete = null;
    public void Tick()
    {
        if (counter > 0f) counter -= Monocle.Engine.DeltaTime;
        else
        {
            Expired = true;
            OnComplete?.Invoke();
            TimerManager.Timers.Remove(this);
        }
    }
    public Timer(float time)
    {
        counter = time;
        TimerManager.Timers.Add(this);
    }
    public Timer(float time, Action onComplete)
    {
        counter = time;
        this.OnComplete = onComplete;
        TimerManager.Timers.Add(this);
    }

    public static explicit operator Timer(Time startFrom) => new Timer(startFrom);
    public static implicit operator float(Timer createFrom) => createFrom.counter;
    public static implicit operator Time(Timer createFrom) => new Time(createFrom);
}
class TimerManager
{
    [OnLoad]
    public static void AddUpdateHook()
    {
        Everest.Events.Level.OnBeforeUpdate += UpdateTimers;
    }
    [OnUnload]
    public static void RemoveUpdateHook()
    {
        Everest.Events.Level.OnBeforeUpdate -= UpdateTimers;
    }
    public static List<Timer> Timers = new();

    private static void UpdateTimers(Level level)
    {
        for (int i = Timers.Count - 1; i >= 0; i--)
            Timers[i].Tick();
    }
}