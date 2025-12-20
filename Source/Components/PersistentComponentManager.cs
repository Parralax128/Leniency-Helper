using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.LeniencyHelper.Components;

abstract class PersistentComponent<T> : Monocle.Component   where T : Entity
{
    protected new T Entity => (T)base.Entity;

    public PersistentComponent() : base(true, false) { }
    public override void Added(Entity entity)
    {
        base.Added(entity);
        if (entity is not T) RemoveSelf();
    }
}

class TweakComponent<T, TweakType> : PersistentComponent<T>     where T : Entity where TweakType : Tweaks.AbstractTweak<TweakType>
{
    static Tweak tweak => Enum.Parse<Tweak>(typeof(TweakType).Name);


    protected static TSetting GetSetting<TSetting>(int index = 0) => TweakData.Tweaks[tweak].GetSetting<TSetting>(index);
    protected static bool TweakEnabled => TweakData.Tweaks[tweak].Enabled;
}

class TweakComponent<TweakType> : PersistentComponent<Player> where TweakType : Tweaks.AbstractTweak<TweakType>
{
    static Tweak tweak => Enum.Parse<Tweak>(typeof(TweakType).Name);

    protected static TSetting GetSetting<TSetting>(int index = 0) => TweakData.Tweaks[tweak].GetSetting<TSetting>(index);
    protected static bool TweakEnabled => TweakData.Tweaks[tweak].Enabled;

    protected Player Player => Entity;
}

static class Manager
{
    // Key = component type | Value = entity type
    static Dictionary<Type, Type> persistentComponentTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypesSafe()).
        Where(x => x.IsClass && !x.IsAbstract && !x.ContainsGenericParameters && IsPersistentComponentType(x)).
        Select(GetTypePair).ToDictionary();
    
    static bool IsPersistentComponentType(Type type)
    {
        if (type.BaseType == null || type.BaseType == typeof(object)) return false;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PersistentComponent<>)) return true;
        
        return IsPersistentComponentType(type.BaseType);
    }
    static KeyValuePair<Type, Type> GetTypePair(Type componentType)
    {
        Type current = componentType;
        while (current != null && current != typeof(object))
        {
            if (current.IsGenericType &&
                current.GetGenericTypeDefinition() == typeof(PersistentComponent<>))
            {
                return new KeyValuePair<Type, Type>(componentType, current.GetGenericArguments()[0]);
            }
            current = current.BaseType;
        }

        throw new ArgumentException($"What the fuck!??\tHow did you manage to have {componentType.Name} not deriving from PersistentComponent<T> ???");
    }


    [OnLoad]
    public static void LoadHooks()
    {
        On.Monocle.Entity.ctor += AddOnCtor;
        On.Monocle.Entity.ctor_Vector2 += AddOnCtorVector2;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Monocle.Entity.ctor -= AddOnCtor;
        On.Monocle.Entity.ctor_Vector2 -= AddOnCtorVector2;
    }

    static void AddOnCtor(On.Monocle.Entity.orig_ctor orig, Entity self)
    {
        orig(self);
        AddEntityComponents(self);
    }
    static void AddOnCtorVector2(On.Monocle.Entity.orig_ctor_Vector2 orig, Entity self, Vector2 pos)
    {
        orig(self, pos);
        AddEntityComponents(self);
    }

    static void AddEntityComponents(Entity entity)
    {
        foreach (Type componentType in persistentComponentTypes.Where(
            pair => pair.Value.IsAssignableFrom(entity.GetType())).Select(pair => pair.Key))
        {
            if (!entity.Components.Any(c => c.GetType() == componentType))
            {
                entity.Add(Activator.CreateInstance(componentType) as Monocle.Component);
            }
                
        }
    }
}