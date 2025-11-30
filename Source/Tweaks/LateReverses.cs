using Monocle;
using Celeste.Mod.LeniencyHelper.Module;
using MonoMod.Cil;
using Microsoft.Xna.Framework;
using Celeste.Mod.Helpers;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class LateReverses : AbstractTweak<LateReverses>
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.SuperJump += LaunchRedirectTimer;
        Everest.Events.Player.OnAfterUpdate += UpdateRedirectTimer;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.SuperJump -= LaunchRedirectTimer;
        Everest.Events.Player.OnAfterUpdate -= UpdateRedirectTimer;
    }

    private static void LaunchRedirectTimer(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        cursor.EmitLdarg0();
        cursor.EmitDelegate(LaunchTimer);

        if(cursor.TryGotoNextBestFit(MoveType.After,
            instr => instr.MatchCallvirt<Player>("get_LiftBoost"),
            instr => instr.MatchCall<Vector2>("op_Addition")))
        {
            cursor.Index--;
            cursor.EmitDup();
            cursor.EmitLdarg0();
            cursor.EmitDelegate(GetRedirectSpeed);
        }
    }
    private static void GetRedirectSpeed(Vector2 liftboost, Player player)
    {
        LeniencyHelperModule.Session.redirectSpeed = (-player.Speed.X + liftboost.X) * (player.Ducking ? 1.25f : 1f);
    }
    private static void LaunchTimer(Player player)
    {
        if((int)player.Facing == Math.Sign(player.Speed.X))
            LeniencyHelperModule.Session.redirectTimer = 0f;
    }
    private static void UpdateRedirectTimer(Player player)
    {   
        var s = LeniencyHelperModule.Session;

        if(s.redirectTimer <= GetSetting<Time>("ReverseTiming"))
        {
            s.redirectTimer += Engine.DeltaTime;
            if (Enabled && s.prevFrameFacing != player.Facing)
            {
                player.Speed.X = s.redirectSpeed;
                s.redirectTimer = 0f;
            }
        }

        s.prevFrameFacing = player.Facing;
    }
}
