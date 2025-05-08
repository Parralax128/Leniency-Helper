using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Celeste;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using FMOD;
using Celeste.Mod.LeniencyHelper.Tweaks;
using Microsoft.Xna.Framework.Content;
using MonoMod.Utils;

namespace Celeste.Mod.LeniencyHelper.UI
{
    public class LHmenuTracker : Component
    {
        public LHmenuTracker(bool active = true, bool visible = false) : base(active, visible)
        {

        }
    }
}
