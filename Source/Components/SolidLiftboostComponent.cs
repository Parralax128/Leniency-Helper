using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

public class SolidLiftboostComponent : Component
{
    public Collider drawRect;
    public float boostSaveTimer = 0f;
    public Vector2 savedLiftSpeed = Vector2.Zero;
    public bool dontSetTimer = false;

    public SolidLiftboostComponent() : base(true, true) { Visible = false; }

    public void OnMove()
    {
        if (!Tweak.SolidBlockboostProtection.Enabled()) return;
        
        if(Entity is Platform p && p.LiftSpeed != Vector2.Zero)
        {
            savedLiftSpeed = (Entity as Platform).LiftSpeed;
            boostSaveTimer = TweakData.Tweaks[Tweak.SolidBlockboostProtection].GetSetting<Time>("MaxSaveTime");
        }
    }
    public void OnSidewaysMove(Vector2 liftspeed)
    {
        savedLiftSpeed = liftspeed;
        boostSaveTimer = TweakData.Tweaks[Tweak.SolidBlockboostProtection].GetSetting<Time>("MaxSaveTime");
    }
}