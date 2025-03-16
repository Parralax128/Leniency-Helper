using System;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using System.Linq;
using Celeste.Mod.LeniencyHelper.UI;
using Celeste.Mod.LeniencyHelper.Tweaks;
using static MonoMod.InlineRT.MonoModRule;

namespace Celeste.Mod.LeniencyHelper.Triggers
{
    [CustomEntity("LeniencyHelper/InputRequiresBlockboostTrigger")]
    public class InputRequiresBlockboostTrigger : Trigger
    {
        public static void LoadHooks()
        {
            On.Monocle.VirtualButton.Update += BindInputsOnUpdate;
            On.Celeste.Player.Update += GetLiftboost;
        }
        public static void UnloadHooks()
        {
            On.Monocle.VirtualButton.Update -= BindInputsOnUpdate;
            On.Celeste.Player.Update -= GetLiftboost;
        }

        private bool oneUse;
        private string flag;

        public enum InputModes
        {
            MoreThan,
            MoreThanOrEqual,
            LessThan,
            LessThanOrEqual,
            IsEqual
        }
       
        public struct BindInfo
        {
            public Inputs bindInput;
            public float liftboostValue;
            public bool vertical;
            public InputModes mode;
            public InputRequiresBlockboostTrigger trigger;
            public BindInfo(Inputs input, float boost, bool vert, InputModes mode, InputRequiresBlockboostTrigger trigger)
            {
                bindInput = input;
                liftboostValue = boost;
                vertical = vert;
                this.mode = mode;
                this.trigger = trigger;
            }
        }

        public BindInfo localBindInfo;
        public InputRequiresBlockboostTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            flag = data.Attr("Flag", "");
            oneUse = data.Bool("OneUse", false);

            localBindInfo = new BindInfo(
                data.Enum("Input", Inputs.Jump),
                data.Float("BlockboostValue", 250f),
                data.Bool("Vertical", false),
                data.Enum("Mode", InputModes.MoreThanOrEqual),
                this);
        }
        public override void Update()
        {
            Collidable = flag == "" || (Scene as Level).Session.GetFlag(flag);
            base.Update();
        }
        public override void OnEnter(Player player)
        {
            PlayerIsInside = true;

            var s = LeniencyHelperModule.Session;
            if (s.BindList is null || !s.BindList.Contains(localBindInfo))
                s.BindList = s.BindList.Append(localBindInfo).ToArray();
        }
        public override void OnLeave(Player player)
        {
            PlayerIsInside = false;
            RemoveFromBindlist();
        }
        private void RemoveFromBindlist()
        {
            var s = LeniencyHelperModule.Session;
            if (s.BindList.Contains(localBindInfo) && s.BindList is not null)
                s.BindList = s.BindList.Where(val => !val.Equals(localBindInfo)).ToArray();
        }

        private static void GetLiftboost(On.Celeste.Player.orig_Update orig, Player self)
        {
            LeniencyHelperModule.Session.playerLiftboost = self.LiftBoost;
            orig(self);
        }
        
        private static bool CheckLiftboost(BindInfo bind)
        {
            var s = LeniencyHelperModule.Session;

            float boost = bind.vertical? s.playerLiftboost.Y : s.playerLiftboost.X;
            bool almostEqual = Math.Abs(Math.Abs(boost) - bind.liftboostValue) < 0.01f;

            bool result = true;
            switch (bind.mode)
            {
                case InputModes.MoreThan: result = Math.Abs(boost) > bind.liftboostValue; break;
                case InputModes.MoreThanOrEqual: result = Math.Abs(boost) > bind.liftboostValue || almostEqual; break;
                case InputModes.LessThan: result = Math.Abs(boost) < bind.liftboostValue; break;
                case InputModes.LessThanOrEqual: result = Math.Abs(boost) < bind.liftboostValue || almostEqual; break;
                case InputModes.IsEqual: result = almostEqual; break;
            }
            if (CustomBufferTime.EnumInputToGameInput(bind.bindInput).Pressed && result && bind.trigger.oneUse)
            {
                bind.trigger.RemoveFromBindlist();
                bind.trigger.RemoveSelf();
            }
            return result;
        }
        private static void BindInputsOnUpdate(On.Monocle.VirtualButton.orig_Update orig, VirtualButton self)
        {
            orig(self);

            if (LeniencyHelperModule.Session is null || LeniencyHelperModule.Session.BindList.Length == 0) return;


            foreach (BindInfo bind in LeniencyHelperModule.Session.BindList)
            {
                if(self.Equals(CustomBufferTime.EnumInputToGameInput(bind.bindInput)))
                    self.consumed = !CheckLiftboost(bind);
            }
        }
    }
}