using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;

namespace Celeste.Mod.LeniencyHelper.Triggers
{
    [CustomEntity("LeniencyHelper/ClearBlockBoostTrigger")]
    public class ClearBlockBoostTrigger : GenericTrigger
    {
        public ClearBlockBoostTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {

        }
        public override void GetOldSettings()
        {
            wasEnabled = LeniencyHelperModule.Session.clearBlockBoostActivated;
        }
        public override void ApplySettings()
        {
            if (LeniencyHelperModule.Session.clearBlockBoostActivated != enabled)
            {
                wasEnabled = LeniencyHelperModule.Session.clearBlockBoostActivated;
                LeniencyHelperModule.Session.clearBlockBoostActivated = enabled;
            }
        }
        public override void UndoSettings()
        {
            LeniencyHelperModule.Session.clearBlockBoostActivated = wasEnabled;
        }
        public static void LoadHooks()
        {
            On.Celeste.Player.Update += ClearBlockBoostUpdate;
            On.Monocle.StateMachine.Update += BanStatesBlockboosting;
        }
        public static void UnloadHooks()
        {
            On.Celeste.Player.Update -= ClearBlockBoostUpdate;
            On.Monocle.StateMachine.Update -= BanStatesBlockboosting;
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
            foreach (Platform booster in player.Scene.Tracker.GetEntities<Platform>())
            {
                Vector2? protectedLiftSpeed;
                DynamicData.For(booster).TryGet<Vector2?>("safeLiftSpeed", out protectedLiftSpeed);
                if (protectedLiftSpeed is not null)
                {
                    DynamicData.For(booster).Set("safeLiftSpeed", Vector2.Zero);
                }
                booster.LiftSpeed = Vector2.Zero;
            }
        }
    }
}
