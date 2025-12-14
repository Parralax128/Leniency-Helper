using System;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.LeniencyHelper.Module;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using System.Linq;
using MonoMod.Core.Platforms;
using YamlDotNet.Serialization.BufferedDeserialization;
namespace Celeste.Mod.LeniencyHelper.Triggers;

[CustomEntity("LeniencyHelper/InputRequiresBlockboostTrigger")]
class InputRequiresBlockboostTrigger : GenericTrigger
{
    [Tweaks.SaveState] static List<BindInfo> BindList = new();
    [Tweaks.SaveState] static Vector2 playerLiftboost;


    public enum BoostComparator
    {
        MoreThan,
        MoreThanOrEqual,
        LessThan,
        LessThanOrEqual,
        IsEqual
    }

    public struct BindInfo
    {
        VirtualButton input;
        float targetLiftspeed;
        bool vertical;
        BoostComparator mode;
        InputRequiresBlockboostTrigger trigger;

        public BindInfo(LeniencyHelperModule.Inputs input, float boost,
            bool vert, BoostComparator mode, InputRequiresBlockboostTrigger trigger)
        {
            this.input = EnumToGameInput(input);
            targetLiftspeed = boost;
            vertical = vert;
            this.mode = mode;
            this.trigger = trigger;
        }

        public bool Check(VirtualButton pressedButton)
        {
            if (!pressedButton.Equals(input)) return true;

            float boost = vertical ? playerLiftboost.Y : playerLiftboost.X;
            bool almostEqual = Math.Abs(boost - targetLiftspeed) < 0.001f;

            bool result = mode switch
            {
                BoostComparator.MoreThan => Math.Abs(boost) > targetLiftspeed,
                BoostComparator.MoreThanOrEqual => Math.Abs(boost) > targetLiftspeed || almostEqual,
                BoostComparator.LessThan => Math.Abs(boost) < targetLiftspeed,
                BoostComparator.LessThanOrEqual => Math.Abs(boost) < targetLiftspeed || almostEqual,
                BoostComparator.IsEqual => almostEqual,
                _ => true
            };
            return result;
        }
        public void RemoveIfOneUse(int index)
        {
            if(trigger.OneUse)
            {
                BindList.RemoveAt(index);
                trigger.RemoveSelf();
            }
        }
    }


    static Hook pressedHook;

    [OnLoad]
    public static void LoadHooks()
    {
        Everest.Events.Player.OnBeforeUpdate += UpdatePlayerLiftboost;
        pressedHook = new Hook(typeof(VirtualButton).GetProperty("Pressed").GetGetMethod(), HookedPressed);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        Everest.Events.Player.OnBeforeUpdate -= UpdatePlayerLiftboost;
        pressedHook.Dispose();
    }
    static void UpdatePlayerLiftboost(Player player) => playerLiftboost = player.LiftBoost.Abs();
    static bool HookedPressed(Func<VirtualButton, bool> orig, VirtualButton self)
    {
        bool pressed = orig(self);
        if (!pressed || BindList.Count == 0) return pressed;


        bool checkBoost = true;

        for (int c = BindList.Count - 1; c >= 0; c--)
        {
            if (BindList[c].Check(self)) BindList[c].RemoveIfOneUse(c);
            else checkBoost = false;
        }

        return checkBoost;
    }

    public static VirtualButton EnumToGameInput(LeniencyHelperModule.Inputs inputName) => inputName switch
    {
        LeniencyHelperModule.Inputs.Jump => Input.Jump,
        LeniencyHelperModule.Inputs.Dash => Input.Dash,
        LeniencyHelperModule.Inputs.Demo => Input.CrouchDash,
        LeniencyHelperModule.Inputs.Grab => Input.Grab,
        _ => null
    };



    BindInfo localBindInfo;
    public InputRequiresBlockboostTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        localBindInfo = new BindInfo(
            data.Enum("Input", LeniencyHelperModule.Inputs.Jump),
            data.Float("BlockboostValue", 250f),
            data.Bool("Vertical", false),
            Parse(data.String("Comparator")) ?? data.Enum("Mode", BoostComparator.MoreThan),
            this);

        static BoostComparator? Parse(string input) => input switch
        {
            ">" => BoostComparator.MoreThan,
            ">=" => BoostComparator.MoreThanOrEqual,
            "<" => BoostComparator.LessThan,
            "<=" => BoostComparator.LessThanOrEqual,
            "=" => BoostComparator.IsEqual,
            _ => null
        };
    }
    protected override void Apply(Player player)
    {
        if (!BindList.Contains(localBindInfo))
            BindList.Add(localBindInfo);
    }
    protected override void Undo(Player player)
    {
        BindList.Remove(localBindInfo);
    }    
}