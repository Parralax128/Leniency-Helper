using Monocle;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper;

public class StampRenderer : Entity
{
    private Vector2 stampPos;
    private MTexture Stamp = GFX.Gui["LeniencyHelper/Parralax/stamp"];

    public StampRenderer(Vector2 stampPos) : base()
    { 
        this.stampPos = stampPos;
        this.Tag = Tags.HUD | Tags.Persistent | Tags.Global;
    }

    public override void Render()
    {
        base.Render();

        if (LeniencyHelperModule.Settings is null || LeniencyHelperModule.Session is null) return;
        
        foreach (string tweak in LeniencyHelperModule.Tweaks)
        {
            if (!LeniencyHelperModule.Session.TweaksByMap.ContainsKey(tweak))
            {
                Log("missing");
                continue;
            }
            if (LeniencyHelperModule.Settings.TweaksByPlayer[tweak] == true && LeniencyHelperModule.Session.TweaksByMap[tweak] == false)
            {
                Stamp.DrawCentered(stampPos, Color.White * 0.15f, 0.35f);
                break;
            }
        }
    }
}