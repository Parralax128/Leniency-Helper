using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Monocle;
using Celeste.Mod.ShroomHelper.Entities;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Module;
using IL.Celeste.Mod.Registry.DecalRegistryHandlers;
using System.Runtime.CompilerServices;
using MonoMod;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.LeniencyHelper.Entities;

//[Tracked(true)]
//[TrackedAs(typeof(Glider))]
[CustomEntity("LeniencyHelper/Entities/UnCeilingBumpableJellyfish")]
public class UnCeilingBumpableJellyfish : Glider
{
    private static ILHook destroyRoutineHook;

    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Glider.OnCollideH += FormatAllAudio;
        IL.Celeste.Glider.OnCollideV += FormatAllAudio;
        IL.Celeste.Glider.OnRelease += FormatAllAudio;
        IL.Celeste.Glider.Update += FormatAllAudio;
        destroyRoutineHook = new ILHook(typeof(Glider).GetMethod("DestroyAnimationRoutine",
            System.Reflection.BindingFlags.NonPublic).GetStateMachineTarget(), FormatAllAudio);

        IL.Celeste.Glider.Update += DisableGroundFriction;
    }

    [OnUnload]
    public static void UnloadHooks()
    {

    }


    private static void FormatAllAudio(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        Func<Instruction, bool> CallAudioPlay = (instr) => 
             instr.MatchCall(typeof(Audio).GetMethod("Play", new Type[] { typeof(string), typeof(Vector2) }));

        while(cursor.TryGotoNext(MoveType.Before, instr => CallAudioPlay(instr)))
        {
            cursor.GotoPrev(MoveType.After, instr => instr.MatchLdstr(out string str));
            cursor.EmitLdarg0();
            cursor.EmitDelegate(FormatAudioEvent);

            cursor.GotoNext(MoveType.After, instr => CallAudioPlay(instr));
        }
    }
    private static string FormatAudioEvent(string eventName, Glider instatnce)
    {
        if(instatnce is UnCeilingBumpableJellyfish customJelly)
        {
            string result = "";
            for (int index = eventName.Length - 1; eventName[index] != '/'; index--)
            {
                result = eventName[index] + result;
            }

            return customJelly.audioDirectory + result;
        }

        return eventName;
    }

    private static void DisableGroundFriction(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        Log(il);
    }

    private static void Log(object o) => Module.LeniencyHelperModule.Log(o);

    private bool prevCollided = false;

    private string audioDirectory;
    private bool disableGroundFriction;

    public UnCeilingBumpableJellyfish(EntityData data, Vector2 offset) : base(data.Position + offset, data.Bool("Floating", true), false)
    {
        audioDirectory = data.String("AudioDirectory", "event:/new_content/game/10_farewell");
        disableGroundFriction = data.Bool("DisableGroundFriction", true);

        this.onCollideV = ModifiedOnCollideV;
    }

    public override void Update()
    {
        base.Update();

        if (!CollideCheck<Solid>(Position - Vector2.UnitY))
            prevCollided = false;
    }

    private void ModifiedOnCollideV(CollisionData data)
    {
        if (prevCollided) return;

        float savedSpeedY = Speed.Y;
        OnCollideV(data);
        if (Speed.Y > savedSpeedY)
            Speed.Y = savedSpeedY;

        prevCollided = true;
    }
}
