using System.Collections.Generic;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper;
static class TimerManager
{
    [OnLoad]
    public static void LoadHook()
    {
        Everest.Events.Level.OnBeforeUpdate += UpdateGameplayTimers;
        IL.Monocle.MInput.Update += UpdateInputTimers;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        Everest.Events.Level.OnBeforeUpdate -= UpdateGameplayTimers;
        IL.Monocle.MInput.Update -= UpdateInputTimers;
    }

    static void UpdateGameplayTimers(Level l) => GameplayTimers.Update();
    static void UpdateInputTimers(ILContext il)
    {
        new ILCursor(il).EmitDelegate(InputTimers.Update);
    }


    public static void Update(this List<Timer> timerList)
    {
        foreach (Timer timer in timerList)
            timer.Tick();
    }


    static List<Timer> GameplayTimers = new();
    static List<Timer> InputTimers = new();

    public static void Add(Timer timer, Timer.Type type)
    {
        if(type == Timer.Type.Gameplay) GameplayTimers.Add(timer);
        else InputTimers.Add(timer);
    }
}