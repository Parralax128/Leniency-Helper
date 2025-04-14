using System;
using System.Linq;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

public class ComponentManager
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Update += ManageComponents;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Update -= ManageComponents;
    }

    private static Component[] GetComponentsToAdd()
    {
        var s = LeniencyHelperModule.Session;

        Component[] result = Array.Empty<Component>();

        if (s.Tweaks["WallCoyoteFrames"].Enabled) result = result.Append(s.WCFcomponent).ToArray();
        if (s.Tweaks["RefillDashInCoyote"].Enabled) result = result.Append(s.RCcomponent).ToArray();

        return result;
    }
    private static Component[] GetComponentsToRemove()
    {
        var s = LeniencyHelperModule.Session;

        Component[] result = Array.Empty<Component>();

        if (!s.Tweaks["WallCoyoteFrames"].Enabled) result = result.Append(s.WCFcomponent).ToArray();
        if (!s.Tweaks["RefillDashInCoyote"].Enabled) result = result.Append(s.RCcomponent).ToArray();

        return result;
    }

    private static void ManageComponents(On.Celeste.Player.orig_Update orig, Player self)
    {
        if(GetComponentsToAdd().Length > 0)
            foreach(Component key in GetComponentsToAdd())
                AddComponentIfHasnt(self, key);

        if (GetComponentsToRemove().Length > 0)
        {
            foreach (Component key in GetComponentsToRemove())
            {
                if (HasComponent(self, key))
                    key.RemoveSelf();
            }
        }      

        orig(self);
    }

    public static void AddComponentIfHasnt(Player player, Component component)
    {
        if (player is null) return;
        if (!HasComponent(player, component))
        {
            player.Add(component);
        }
    }
    public static bool HasComponent(Player player, Component component)
    {
        if (player is null || player.Components.Count <= 0) return false;
        
        foreach(Component item in player.Components)
        {
            if (item.GetType().Name == component.GetType().Name) return true;
        }
        return false;
    }
}