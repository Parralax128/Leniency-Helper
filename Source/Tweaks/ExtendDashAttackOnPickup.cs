namespace Celeste.Mod.LeniencyHelper.Tweaks;

class ExtendDashAttackOnPickup : AbstractTweak<ExtendDashAttackOnPickup> 
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Pickup += ExtendAttack;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Pickup -= ExtendAttack;
    }
    
    static bool ExtendAttack(On.Celeste.Player.orig_Pickup orig, Player self, Holdable holdable)
    {
        bool result = orig(self, holdable);

        if (Enabled && result && self.dashAttackTimer > 0f) 
            self.dashAttackTimer += GetSetting<Time>();

        return result;
    }
}