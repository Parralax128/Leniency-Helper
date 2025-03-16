using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using Monocle;
using Celeste.Mod.LeniencyHelper.Tweaks;

namespace Celeste.Mod.LeniencyHelper.TweakTriggers;

[CustomEntity("LeniencyHelper/Triggers/CustomBufferTime")]
public class CustomBufferTimeTrigger : GenericTweakTrigger
{
    private Dictionary<Inputs, float> dataBuffers = CustomBufferTime.GetVanillaBuffers();
    private Dictionary<Inputs, float> oldBuffers;
    private bool countInFrames = false;
    public CustomBufferTimeTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        tweakName = "CustomBufferTime";
        foreach(Inputs input in dataBuffers.Keys)
        {
            dataBuffers[input] = data.Float($"{input}BufferTime", 0.08f);
        }
        countInFrames = data.Bool("CountTimeInFrames", false);
    }

    public override void ApplySettings()
    {
        base.ApplySettings();

        foreach (Inputs input in dataBuffers.Keys)
        {
            LeniencyHelperModule.Session.mapBuffers[input] = 
                (countInFrames ? dataBuffers[input] / Engine.FPS : dataBuffers[input]);
        }

        CustomBufferTime.UpdateCustomBuffers();
    }
    public override void GetOldSettings()
    {
        base.GetOldSettings();
        oldBuffers = new Dictionary<Inputs, float>(LeniencyHelperModule.Session.mapBuffers);
    }
    public override void UndoSettings()
    {
        base.UndoSettings();
        LeniencyHelperModule.Session.mapBuffers = new Dictionary<Inputs, float>(oldBuffers);
        CustomBufferTime.UpdateCustomBuffers();
    }
}
