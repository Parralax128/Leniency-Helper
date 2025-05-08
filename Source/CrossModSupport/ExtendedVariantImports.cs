using MonoMod.ModInterop;
using System;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.CrossModSupport;

[ModImportName("ExtendedVariantMode")]
public static class ExtendedVariantImports
{
    public static Func<string, object> GetCurrentVariantValue;
    public static Vector2 CornerCorrection
    { 
        get
        {
            if (GetCurrentVariantValue != null)
            {
                return Vector2.One * (int)GetCurrentVariantValue.Invoke("CornerCorrection");
            }

            return new Vector2(4, 2);
        }
    }
}