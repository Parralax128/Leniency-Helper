using Celeste.Mod.LeniencyHelper.Tweaks;
using IL.MonoMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.LeniencyHelper.UI
{
    public class EnumSlider : TextMenu.Option<int>
    {
        public Action<int> OnValueChange
        {
            get
            {
                return base.OnValueChange;
            }
            set
            {
                base.OnValueChange = value;
            }
        }

        public EnumSlider(string label, Dictionary<string, int> values) : base(label)
        {
            foreach (string key in values.Keys)
            {
                Add(key, values[key]);
            }
        }
    }
}
