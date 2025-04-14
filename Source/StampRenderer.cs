using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper;

[Tracked(false)]
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
        
        foreach (string tweak in LeniencyHelperModule.tweakList)
        {
            if (LeniencyHelperModule.Session.Tweaks[tweak].playerValue == true && LeniencyHelperModule.Session.Tweaks[tweak].MapEnabled == false)
            {
                Stamp.DrawCentered(stampPos, Color.White * 0.15f, 0.35f);
                break;
            }
        }
    }
}