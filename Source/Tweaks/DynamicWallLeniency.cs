using Monocle;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class DynamicWallLeniency : AbstractTweak<DynamicWallLeniency>
{
    // to avoid unnecesarry IL hooks, the leniency is applyed in IceWallIncreaseWallLeniency tweak

    public static int GetDynamicLeniency(Player player, int @default)
    {
        if (!Enabled) return @default;

        float biggestSpeed = Math.Abs(player.Speed.X);
        if (player.DashAttacking) biggestSpeed = Math.Max(Math.Abs(player.Speed.X), Math.Abs(player.beforeDashSpeed.X));

        return Math.Max((int)(biggestSpeed * GetTime("wallLeniencyTiming")), @default);
    }
}