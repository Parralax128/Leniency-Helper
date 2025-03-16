using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste;
using MonoMod.Utils;
using MonoMod.ModInterop;

namespace Celeste.Mod.LeniencyHelper.CrossModSupport
{
    [ModImportName("GravityHelper")]
    public static class GravityHelperImports
    {
        public static Func<int> GetPlayerGravity;

        public static int currentGravity => GetPlayerGravity is not null ? -GetPlayerGravity.Invoke() * 2 + 1 : 1;
        //  1 is normal, -1 is inverted
    }
}