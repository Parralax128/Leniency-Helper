using MonoMod.Cil;
using Microsoft.Xna.Framework;
using Celeste.Mod.Helpers;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class LateReverses : AbstractTweak<LateReverses>
{
    [SaveState] static float redirectSpeed;
    [SaveState] static Facings prevFrameFacing;
    static Timer RedirectTimer = new();

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

    static void LaunchRedirectTimer(ILContext il)
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

        static void GetRedirectSpeed(Vector2 liftboost, Player player) =>
            redirectSpeed = (-player.Speed.X + liftboost.X) * (player.Ducking ? 1.25f : 1f);
        static void LaunchTimer(Player player)
        {
            if ((int)player.Facing == Math.Sign(player.Speed.X))
                RedirectTimer.Launch(GetSetting<Time>());
        }
    }
    
    
    static void UpdateRedirectTimer(Player player)
    {
        if(RedirectTimer)
        {
            if (Enabled && prevFrameFacing != player.Facing)
            {
                player.Speed.X = redirectSpeed;
                RedirectTimer.Abort();
            }
        }

        prevFrameFacing = player.Facing;
    }
}