using System;
using Celeste.Mod.LeniencyHelper;
using System.Collections.Generic;
using Monocle;
using YamlDotNet.Serialization.BufferedDeserialization;
using System.ComponentModel.Design;
using YamlDotNet.Serialization.ObjectFactories;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper;
static class TimerManager
{
    [OnLoad]
    public static void LoadHook()
    {
        Everest.Events.Level.OnBeforeUpdate += UpdateStaticTimers;
        On.Monocle.Engine.Update += UpdateStaticUnfrozen;
        Everest.Events.LevelLoader.OnLoadingThread += AddEntity;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        Everest.Events.Level.OnBeforeUpdate -= UpdateStaticTimers;
        On.Monocle.Engine.Update -= UpdateStaticUnfrozen;
        Everest.Events.LevelLoader.OnLoadingThread -= AddEntity;
    }

    private static void AddEntity(Level level) => level.Add(new TimerUpdater());
    private static void UpdateStaticTimers(Level l) => StaticTimers.Update();
    
    private static void UpdateStaticUnfrozen(On.Monocle.Engine.orig_Update orig, Engine self, GameTime time)
    {
        StaticUnfrozenTimers.Update();
        orig(self, time);
    }

    public static int IgnoreStates = 1;
    public static int IgnoreFreeze = 2;

    public static void Update(this List<Timer> timerList)
    {
        for (int i = timerList.Count - 1; i >= 0; i--)
            timerList[i].Tick();
    }


    private static List<Timer> StaticTimers = new();
    private static List<Timer> StaticUnfrozenTimers = new();

    public static void Add(Timer timer, int tag)
    {
        if ((tag & IgnoreStates) == 0) TimerUpdater.Add(timer, (tag & IgnoreFreeze) != 0);
        else
        {
            if((tag & IgnoreFreeze) == 0) StaticTimers.Add(timer);
            else StaticUnfrozenTimers.Add(timer);
        }
    }

    private class TimerUpdater : Entity
    {
        static TimerUpdater Instance;

        private List<Timer> timers = new();
        private List<Timer> unfrozenTimers = new();

        public TimerUpdater()
        {
            AddTag(Tags.Persistent);
            AddTag(Tags.Global);

            Instance = this;
        }

        public static void Add(Timer timer, bool unfrozen = false)
        {
            if(unfrozen) Instance.unfrozenTimers.Add(timer);
            else Instance.timers.Add(timer);
        }
        public override void Update()
        {
            timers.Update();
            unfrozenTimers.Update();
        }
    }
}