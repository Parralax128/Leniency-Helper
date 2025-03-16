using Monocle;
using System;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class DynamicWallLeniency
{
    //  to avoid unnecesarry IL hooks, its effect is applyed in IceWallIncreaseWallLeniency tweak

    public static int GetDynamicLeniency(Player player, int defaultValue)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["DynamicWallLeniency"]) return defaultValue;

        var s = LeniencyHelperModule.Settings;
        float biggestSpeed = Math.Abs(player.Speed.X);

        if (player.DashAttacking) biggestSpeed = Math.Max(Math.Abs(player.Speed.X), Math.Abs(player.beforeDashSpeed.X));

        return Math.Max((int)(biggestSpeed * ((bool)s.GetSetting("DynamicWallLeniency", "countWallTimingInFrames") ?
            (float)s.GetSetting("DynamicWallLeniency", "wallLeniencyTiming") / Engine.FPS :
            (float)s.GetSetting("DynamicWallLeniency", "wallLeniencyTiming"))), defaultValue);

    }
}