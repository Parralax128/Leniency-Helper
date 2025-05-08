using System;
using MonoMod.ModInterop;

namespace Celeste.Mod.LeniencyHelper.CrossModSupport;

[ModImportName("GravityHelper")]
public static class GravityHelperImports
{
    public static Func<int> GetPlayerGravity;

    public static int currentGravity => GetPlayerGravity != null ? -GetPlayerGravity.Invoke() * 2 + 1 : 1;
    //  1 is normal, -1 is inverted
}