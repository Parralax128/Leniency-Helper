using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.LeniencyHelper.Module;

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
        
        foreach (string tweak in LeniencyHelperModule.TweakList)
        {
            if (LeniencyHelperModule.Settings.PlayerTweaks[tweak] == true 
                && (LeniencyHelperModule.Session.UseController[tweak]?
                LeniencyHelperModule.Session.ControllerTweaks[tweak] : LeniencyHelperModule.Session.TriggerTweaks[tweak]) == false)
            {
                Stamp.DrawCentered(stampPos, Color.White * 0.15f, 0.35f);
                break;
            }
        }
    }
}