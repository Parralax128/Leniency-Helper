using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class ConsistentWallboosters : AbstractTweak<ConsistentWallboosters>
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.ClimbUpdate += ModifyAcceleration;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.ClimbUpdate -= ModifyAcceleration;
    }

    private static void ModifyAcceleration(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        VariableDefinition targetSpeed = new VariableDefinition(il.Import(typeof(float)));
        il.Body.Variables.Add(targetSpeed);

        ILLabel skipInstantAcc = il.DefineLabel();
        ILLabel skipWJ = il.DefineLabel();

        if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt<Player>("WallJump")))
        {
            if(cursor.TryGotoPrev(MoveType.After,
                instr => instr.MatchCallvirt<VirtualButton>("get_Pressed"),
                instr => instr.MatchBrfalse(out skipWJ)))
            {
                cursor.EmitLdarg0();
                cursor.EmitDelegate(EdgeJumpCheck);
                cursor.EmitBrfalse(skipWJ);
            }
        }

        if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("WallBoosterCheck")))
        {
            if (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCall(typeof(Calc).GetMethod("Approach",
                new Type[] { typeof(float), typeof(float), typeof(float) }))))
            {
                cursor.GotoPrev(MoveType.Before, instr => instr.MatchLdcR4(600f));
                cursor.EmitDup();
                cursor.EmitStloc(targetSpeed);

                cursor.GotoNext(MoveType.Before, instr => instr.MatchCall(typeof(Engine).GetMethod("get_DeltaTime")));
                cursor.EmitDelegate(GetNewAcceleration);

                cursor.GotoNext(MoveType.After, instr => instr.MatchStfld<Vector2>("Y"));
                
                cursor.EmitDelegate(InstantAccelerationEnabled);
                cursor.EmitBrfalse(skipInstantAcc);
                {
                    cursor.EmitLdarg0();
                    cursor.EmitLdflda(typeof(Player).GetField("Speed"));
                    cursor.EmitLdloc(targetSpeed);
                    cursor.EmitStfld(typeof(Vector2).GetField("Y"));
                }
                cursor.MarkLabel(skipInstantAcc);


                if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCall(typeof(Math).GetMethod("Max",
                    new Type[] { typeof(float), typeof(float) }))))
                {
                    cursor.EmitDup();
                    cursor.EmitStloc(targetSpeed);

                    cursor.Index++;

                    cursor.EmitLdloc(targetSpeed);
                    cursor.EmitDelegate(GetConsistentBlockboost);
                }
            }
        }
    }

    private static bool EdgeJumpCheck(Player player)
    {
        if (!Enabled || !GetSetting<bool>("BufferableMaxjumps")) return true;
        
        Vector2 savedPos = player.Position;
        player.Position += player.Speed * Input.Jump.bufferCounter;

        if (player.WallBoosterCheck() != null && player.WallJumpCheck((int)player.Facing))
        {
            player.Position = savedPos;
            return true;
        }

        player.Position = savedPos;

        player.Position += player.Speed * Engine.DeltaTime; // next frame position
        bool noWallboosterNextFrame = player.WallBoosterCheck() == null || !player.WallJumpCheck((int)player.Facing);
        player.Position = savedPos;

        return noWallboosterNextFrame;
    }

    private static bool InstantAccelerationEnabled() =>
         Enabled && GetSetting<bool>("InstantAcceleration");

    private static float GetNewAcceleration(float orig)
    {
        if (!Enabled || GetSetting<bool>("InstantAcceleration"))
            return orig;

        return GetSetting<int>("CustomAcceleration") * 60f;
    }

    private static float GetConsistentBlockboost(float orig, float target) => 
        Enabled && GetSetting<bool>("ConsistentBlockboost") ? target : orig;         
}