using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Tweaks.Attributes;
class TempContainerEntity : Entity
{
    public static TempContainerEntity Instance { get; private set; }

    List<List<object>> temps;

    public static Dictionary<Tweak, List<object>> Pending = new();
   
    public object Get(Tweak tweak, int index) => temps[(int)tweak][index];
    public void Set(Tweak tweak, int index, object value) => temps[(int)tweak][index] = value;

    public TempContainerEntity() : base()
    {
        if(Instance != null) temps = Instance.temps;
        else if (Pending != null && Pending.Count > 0)
        {
            temps = new();
            foreach(Tweak tweak in Module.LeniencyHelperModule.TweakList)
            {
                if (Pending.ContainsKey(tweak))
                    temps.Add(Pending[tweak]);
                
                else temps.Add(null);
            }
            Pending.Clear();
            Pending = null;
        }

        Instance = this;
    }
}