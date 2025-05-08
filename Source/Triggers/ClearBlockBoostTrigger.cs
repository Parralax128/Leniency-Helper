using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Triggers
{
    [CustomEntity("LeniencyHelper/ClearBlockBoostTrigger")]
    public class ClearBlockBoostTrigger : GenericTrigger
    {
        [OnLoad]
        public static void LoadHooks()
        {
            On.Celeste.Player.Update += ClearBlockBoostUpdate;
            On.Monocle.StateMachine.Update += BanStatesBlockboosting;
        }
        [OnUnload]
        public static void UnloadHooks()
        {
            On.Celeste.Player.Update -= ClearBlockBoostUpdate;
            On.Monocle.StateMachine.Update -= BanStatesBlockboosting;
        }

        public ClearBlockBoostTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {

        }

        public override void ApplySettings()
        {
            LeniencyHelperModule.Session.clearBlockBoostActivated = true;
        }
        public override void UndoSettings()
        {
            LeniencyHelperModule.Session.clearBlockBoostActivated = false;
        }
        private static void BanStatesBlockboosting(On.Monocle.StateMachine.orig_Update orig, StateMachine self)
        {
            if (LeniencyHelperModule.Session.clearBlockBoostActivated)
                SetLiftSpeedToZero(self.Entity as Player);

            orig(self);
        }
        private static void ClearBlockBoostUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            if (LeniencyHelperModule.Session.clearBlockBoostActivated)
            {
                SetLiftSpeedToZero(self);
                self.ResetLiftSpeed();
            }
            orig(self);
        }
        private static void SetLiftSpeedToZero(Player player)
        {
            foreach (Platform blockBooster in player.Scene.Tracker.GetEntities<Platform>())
            {
                Vector2? protectedLiftSpeed;
                DynamicData.For(blockBooster).TryGet("safeLiftSpeed", out protectedLiftSpeed);
                if (protectedLiftSpeed is not null)
                {
                    DynamicData.For(blockBooster).Set("safeLiftSpeed", Vector2.Zero);
                }
                blockBooster.LiftSpeed = Vector2.Zero;
            }
        }
    }
}
