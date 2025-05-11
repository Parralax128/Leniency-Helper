using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class ManualDreamhyperLeniency : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.DashUpdate += RemoveDashDirCheck;
    }

    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.DashUpdate -= RemoveDashDirCheck;
    }

    private static void Log(object o) => Module.LeniencyHelperModule.Log(o);
    private static void RemoveDashDirCheck(ILContext il)
    {
        Log(il);
    }

}
