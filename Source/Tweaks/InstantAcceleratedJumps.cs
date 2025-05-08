using MonoMod.Cil;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class InstantAcceleratedJumps : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.Jump += RecieveSpeedOnStart;
        IL.Celeste.Player.Bounce += RecieveSpeedOnStart;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.Jump -= RecieveSpeedOnStart;
        IL.Celeste.Player.Bounce -= RecieveSpeedOnStart;
        //asljdfadsjfalsfjdslkasdfasfdassdfss
    }

    private static void RecieveSpeedOnStart(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        cursor.EmitLdarg0();
        cursor.EmitDelegate(RecieveWalkSpeed);
    }
    private static void RecieveWalkSpeed(Player player)
    {
        if (Enabled("InstantAcceleratedJumps") && Math.Abs(player.Speed.X) < 90f && player.moveX != 0)
        {
            player.Speed.X = player.moveX * 90f;
        }
    }
}