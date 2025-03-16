using Monocle;
using System;
using System.Collections.Generic;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class CustomBufferTime
{
    public static Dictionary<Inputs, float> prevFrameBuffers = GetVanillaBuffers();
    
    public static Dictionary<Inputs, float> GetVanillaBuffers()
    {
        return new Dictionary<Inputs, float>
        {
            { Inputs.Jump, 0.08f },
            { Inputs.Dash, 0.08f },
            { Inputs.Demo, 0.08f }
        };
    }

    public static Dictionary<Inputs, float> newBuffers = new Dictionary<Inputs, float>();
    public static Inputs[] VanillaInputs = { Inputs.Jump, Inputs.Dash, Inputs.Demo };

    public static void LoadHooks()
    {
        On.Celeste.Player.Update += ControlBuffers;
        On.Celeste.Player.ctor += CustomBuffersOnRespawn;
    }
    public static void UnloadHooks() 
    {
        On.Celeste.Player.Update -= ControlBuffers;
        On.Celeste.Player.ctor -= CustomBuffersOnRespawn;
    }


    public static void UpdateCustomBuffers(int newIndex)
    {
        if (newIndex == 1)
        {
            newBuffers.Clear();
            foreach (Inputs input in LeniencyHelperModule.Settings.buffers.Keys)
                newBuffers.Add(input, GetActualPlayerBuffer(input));
        }

        else if (newIndex == 0 && LeniencyHelperModule.Session.TweaksByMap["CustomBufferTime"])
            newBuffers = new Dictionary<Inputs, float>(LeniencyHelperModule.Session.mapBuffers);

        else SetDefaultBuffers();
    }
    public static void UpdateCustomBuffers()
    {
        int newIndex = GetIndex();
        if (newIndex == 1)
        {
            newBuffers.Clear();
            foreach (Inputs input in LeniencyHelperModule.Settings.buffers.Keys)
                newBuffers.Add(input, GetActualPlayerBuffer(input));
        }

        else if (newIndex == 0 && LeniencyHelperModule.Session.TweaksByMap["CustomBufferTime"])
            newBuffers = new Dictionary<Inputs, float>(LeniencyHelperModule.Session.mapBuffers);

        else SetDefaultBuffers();
    }

    public static VirtualButton EnumInputToGameInput(Inputs inputName)
    {
        switch(inputName)
        {
            case Inputs.Jump: return Input.Jump;
            case Inputs.Dash: return Input.Dash;
            case Inputs.Demo: return Input.CrouchDash;
        }
        return null;
    }
    public static void SetPlayerBuffers()
    {
        foreach (Inputs input in VanillaInputs)
        {
            newBuffers.Add(input, GetActualPlayerBuffer(input));
        }
    }
    private static void SetDefaultBuffers() 
    {
        var s = LeniencyHelperModule.Session;
        foreach (Inputs input in s.defaultBuffers.Keys)
        {
            EnumInputToGameInput(input).BufferTime = s.defaultBuffers[input];
        }
    }
    private static void ControlBuffers(On.Celeste.Player.orig_Update orig, Player self)
    {
        if (newBuffers.Count > 0 && LeniencyHelperModule.Session.TweaksEnabled["CustomBufferTime"])
            foreach (Inputs input in newBuffers.Keys)
                EnumInputToGameInput(input).BufferTime = newBuffers[input]; // applying new buffers


        foreach (Inputs input in VanillaInputs)
        {
            if (prevFrameBuffers[input] != EnumInputToGameInput(input).BufferTime &&  // if sth other from Leniency helper changed buffers - 
                (newBuffers.Count == 0 || !newBuffers.Keys.Contains(input)))          // setting new default value
            {
                LeniencyHelperModule.Session.defaultBuffers[input] = EnumInputToGameInput(input).BufferTime;

                if (LeniencyHelperModule.Settings.TweaksByPlayer["CustomBufferTime"] == true) // if custom buffers enabled in settings - returning them back
                    UpdateCustomBuffers(1);
            }

            prevFrameBuffers[input] = EnumInputToGameInput(input).BufferTime;
        }
        if (newBuffers.Count > 0) newBuffers.Clear();

        orig(self);
    }
    private static float GetActualPlayerBuffer(Inputs input)
    {
        return ((bool)LeniencyHelperModule.Settings.GetSetting("CustomBufferTime", "countBufferTimeInFrames") ?
            LeniencyHelperModule.Settings.buffers[input] / Engine.FPS : LeniencyHelperModule.Settings.buffers[input]);
    }

    private static void CustomBuffersOnRespawn(On.Celeste.Player.orig_ctor orig, Player self, Vector2 pos, PlayerSpriteMode mode)
    {
        orig(self, pos, mode);

        if (LeniencyHelperModule.Session.TweaksEnabled["CustomBufferTime"]) UpdateCustomBuffers();
    }
    public static int GetIndex()
    {
        if (LeniencyHelperModule.Settings.TweaksByPlayer["CustomBufferTime"] != null)
            return (LeniencyHelperModule.Settings.TweaksByPlayer["CustomBufferTime"] == true) ? 1 : 2;
        else return 0;
    }
}

