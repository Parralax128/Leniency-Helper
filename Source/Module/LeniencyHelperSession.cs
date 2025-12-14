using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static Celeste.Mod.LeniencyHelper.Triggers.InputRequiresBlockboostTrigger;

namespace Celeste.Mod.LeniencyHelper.Module;

class LeniencyHelperSession : EverestModuleSession
{
    public int wjDistR { get; set; } = 3;
    public int wjDistL { get; set; } = 3;
}