using Monocle;
using System.Collections;
using System.Linq.Expressions;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class FrozenReverses
{
    public static void LoadHooks()
    {
        On.Celeste.Player.SuperJump += StartFreezeCoroutine;
    }
    public static void UnloadHooks() 
    {
        On.Celeste.Player.SuperJump -= StartFreezeCoroutine;
    }

    private static bool tryingToSuper = false;
    private static float actualFreezeTime
    {
        get
        {
            var s = LeniencyHelperModule.Settings;
            if ((bool)s.GetSetting("FrozenReverses", "countReversedInFrames"))
                return (float)s.GetSetting("FrozenReverses", "reversedFreezeTime") / Engine.FPS;
            else return (float)s.GetSetting("FrozenReverses", "reversedFreezeTime");
        }
    }

    private static void StartFreezeCoroutine(
        On.Celeste.Player.orig_SuperJump orig, Player self)
    {
        if (!LeniencyHelperModule.Session.TweaksEnabled["FrozenReverses"])
        {
            orig(self);
            return;
        }
        var s = LeniencyHelperModule.Session;

        s.origSuperJump = orig;
        if (!tryingToSuper)
        {
            s.playerDucked = self.Ducking;
            Celeste.Freeze(actualFreezeTime);
            tryingToSuper = true;
            self.Add(new Coroutine(FreezeReverseCoroutine(self)));
        }
    }
    private static IEnumerator FreezeReverseCoroutine(Player player)
    {
        var s = LeniencyHelperModule.Session;
        player.Ducking = s.playerDucked;
        s.origSuperJump(player);
        tryingToSuper = false;
        yield return null;
    }
}