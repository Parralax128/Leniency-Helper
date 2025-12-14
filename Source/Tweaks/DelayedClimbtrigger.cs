using Celeste.Mod.LeniencyHelper.Components;
using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class DelayedClimbtrigger : AbstractTweak<DelayedClimbtrigger>
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Solid.ctor += AddComponent;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Solid.ctor -= AddComponent;
    }

    public static bool useOrigCheck;

    static void AddComponent(On.Celeste.Solid.orig_ctor orig, Solid self, Vector2 position, float width, float height, bool safe)
    {
        orig(self, position, width, height, safe);
        if(self is not SolidTiles) self.Add(new DelayedClimbtriggerComponent());
    }

    public static Player GetClimbtriggeringPlayer(Solid solid, Player player = null)
    {
        if (!Enabled || useOrigCheck) return null; // preventing player getting blockboosts && being moved by the solid

        var component = solid.Get<DelayedClimbtriggerComponent>();
        if (component == null) return null;

        return solid.Get<DelayedClimbtriggerComponent>()?.ClimbtriggerTimer ?
            player ?? LeniencyHelperModule.GetPlayer(solid.Scene) : null;
    }
}
