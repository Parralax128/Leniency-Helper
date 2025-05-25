﻿using System;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.LeniencyHelper.Module;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[CustomEntity("LeniencyHelper/InputRequiresBlockboostTrigger")]
public class InputRequiresBlockboostTrigger : Trigger
{
    private static Hook pressedHook;

    [OnLoad]
    public static void LoadHooks()
    {
        LeniencyHelperModule.BeforePlayerUpdate += GetLiftboost;
        pressedHook = new Hook(typeof(VirtualButton).GetProperty("Pressed").GetGetMethod(), HookedPressed);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        LeniencyHelperModule.BeforePlayerUpdate -= GetLiftboost;

        pressedHook.Dispose();
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
        public LeniencyHelperModule.Inputs bindInput;
        public float targetLiftspeed;
        public bool vertical;
        public InputModes mode;
        public InputRequiresBlockboostTrigger trigger;
        public BindInfo(LeniencyHelperModule.Inputs input, float boost,
            bool vert, InputModes mode, InputRequiresBlockboostTrigger trigger)
        {
            bindInput = input;
            targetLiftspeed = boost;
            vertical = vert;
            this.mode = mode;
            this.trigger = trigger;
        }
    }

    public BindInfo localBindInfo;
    public static List<BindInfo> bindsToRemove;
    public InputRequiresBlockboostTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        flag = data.Attr("Flag", "");
        oneUse = data.Bool("OneUse", false);

        localBindInfo = new BindInfo(
            data.Enum("Input", LeniencyHelperModule.Inputs.Jump),
            data.Float("BlockboostValue", 250f),
            data.Bool("Vertical", false),
            data.Enum("Mode", InputModes.MoreThanOrEqual),
            this);

        bindsToRemove = new List<BindInfo>();
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
        if (!s.BindList.Contains(localBindInfo))
            s.BindList.Add(localBindInfo);
    }
    public override void OnLeave(Player player)
    {
        PlayerIsInside = false;
        RemoveFromBindlist();
    }
    private void RemoveFromBindlist()
    {
        var s = LeniencyHelperModule.Session;
        if (s.BindList.Contains(localBindInfo))
            s.BindList.Remove(localBindInfo);
    }

    private static void GetLiftboost(Player player)
    {
        LeniencyHelperModule.Session.playerLiftboost = player.LiftBoost.Abs();
    }

    private static bool CheckLiftboost(BindInfo bind, bool origPressed)
    {
        float boost = bind.vertical ? LeniencyHelperModule.Session.playerLiftboost.Y : LeniencyHelperModule.Session.playerLiftboost.X;
        bool almostEqual = Math.Abs(boost - bind.targetLiftspeed) < 0.01f;

        bool result = true;
        switch (bind.mode)
        {
            case InputModes.MoreThan: result = Math.Abs(boost) > bind.targetLiftspeed; break;
            case InputModes.MoreThanOrEqual: result = Math.Abs(boost) > bind.targetLiftspeed || almostEqual; break;
            case InputModes.LessThan: result = Math.Abs(boost) < bind.targetLiftspeed; break;
            case InputModes.LessThanOrEqual: result = Math.Abs(boost) < bind.targetLiftspeed || almostEqual; break;
            case InputModes.IsEqual: result = almostEqual; break;
        }
        if (origPressed && result && bind.trigger.oneUse)
        {
            bindsToRemove.Add(bind);
            bind.trigger.RemoveSelf();
        }

        return result;
    }

    private static bool HookedPressed(Func<VirtualButton, bool> orig, VirtualButton self)
    {
        bool origResult = orig(self);
        if (LeniencyHelperModule.Session == null || LeniencyHelperModule.Session.BindList.Count == 0)
            return origResult;


        bool lockInput = false;
        foreach (BindInfo bind in LeniencyHelperModule.Session.BindList)
        {
            if (self.Equals(EnumInputToGameInput(bind.bindInput)))
                lockInput = lockInput || !CheckLiftboost(bind, origResult);
        }
        foreach (BindInfo bindToRemove in bindsToRemove)
        {
            LeniencyHelperModule.Session.BindList.Remove(bindToRemove);
        }
        bindsToRemove.Clear();

        return origResult && !lockInput;
    }
    public static VirtualButton EnumInputToGameInput(LeniencyHelperModule.Inputs inputName)
    {
        switch (inputName)
        {
            case LeniencyHelperModule.Inputs.Jump: return Input.Jump;
            case LeniencyHelperModule.Inputs.Dash: return Input.Dash;
            case LeniencyHelperModule.Inputs.Demo: return Input.CrouchDash;
        }
        return null;
    }
}