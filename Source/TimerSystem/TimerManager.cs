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
        Everest.Events.LevelLoader.OnLoadingThread += AddTimerUpdaterEntity;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        Everest.Events.Level.OnBeforeUpdate -= UpdateStaticTimers;
        On.Monocle.Engine.Update -= UpdateStaticUnfrozen;
        Everest.Events.LevelLoader.OnLoadingThread -= AddTimerUpdaterEntity;
    }

    static void AddTimerUpdaterEntity(Level level) => level.Add(new TimerUpdater());
    static void UpdateStaticTimers(Level l) => StaticTimers.Update();
    
    static void UpdateStaticUnfrozen(On.Monocle.Engine.orig_Update orig, Engine self, GameTime time)
    {
        StaticUnfrozenTimers.Update();
        if (Engine.Scene is Level level)
            TimerUpdater.Instance.UnfrozenTimers?.Update();

        orig(self, time);
    }

    public const int IgnoreStates = 1;
    public const int IgnoreFreeze = 2;

    public static void Update(this List<Timer> timerList)
    {
        foreach (Timer timer in timerList)
            timer.Tick();
    }


    static List<Timer> StaticTimers = new();
    static List<Timer> StaticUnfrozenTimers = new();

    public static void Add(Timer timer, int tag)
    {
        if ((tag & IgnoreStates) == 0) TimerUpdater.Add(timer, (tag & IgnoreFreeze) != 0);
        else
        {
            if((tag & IgnoreFreeze) == 0) StaticTimers.Add(timer);
            else StaticUnfrozenTimers.Add(timer);
        }
    }

    class TimerUpdater : Entity
    {
        public static TimerUpdater Instance;

        static List<Timer> toAdd = new();
        static List<Timer> toAddUnfrozen = new();

        List<Timer> timers = new();
        public List<Timer> UnfrozenTimers = new();

        public TimerUpdater()
        {
            AddTag(Tags.Global);
            AddTag(Tags.TransitionUpdate);
            AddTag(Tags.FrozenUpdate);
            AddTag(Tags.Persistent);

            if(Instance != null)
            {
                timers.AddRange(Instance.timers);
                UnfrozenTimers.AddRange(Instance.UnfrozenTimers);
            }

            Instance = this;

            timers.AddRange(toAdd);
            toAdd.Clear();

            UnfrozenTimers.AddRange(toAddUnfrozen);
            toAddUnfrozen.Clear();
        }

        public static void Add(Timer timer, bool unfrozen = false)
        {
            if (Instance != null)
            {
                if (unfrozen) Instance.UnfrozenTimers.Add(timer);
                else Instance.timers.Add(timer);
            }
            else
            {
                if(unfrozen) toAddUnfrozen.Add(timer);
                else toAdd.Add(timer);
            }
        }
        public override void Update()
        {
            timers.Update();
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            
            if(Instance != null) 
                Instance = null;
        }
    }
}