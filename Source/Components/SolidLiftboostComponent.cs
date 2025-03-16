using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

public class SolidLiftboostComponent : Component
{
    public Collider drawRect;
    private float BoostSaveTime
    {
        get
        {
            return (bool)LeniencyHelperModule.Settings.GetSetting("SolidBlockboostProtection", "countSolidBoostSaveTimeInFrames") ?
                (float)LeniencyHelperModule.Settings.GetSetting("SolidBlockboostProtection", "bboostSaveTime")/Engine.FPS :
                (float)LeniencyHelperModule.Settings.GetSetting("SolidBlockboostProtection", "bboostSaveTime");
        }
    }
    public float boostSaveTimer = 0f;
    public Vector2 savedLiftSpeed = Vector2.Zero;
    public bool dontSetTimer = false;

    public SolidLiftboostComponent() : base(true, true) { Visible = true; }

    public void OnMove()
    {
        if (LeniencyHelperModule.Session.TweaksEnabled["SolidBlockboostProtection"])
        {
            if(Entity is Platform p && p.LiftSpeed != Vector2.Zero)
            {
                savedLiftSpeed = (Entity as Platform).LiftSpeed;
                boostSaveTimer = BoostSaveTime;
            }
        }
    }
    public void OnSidewaysMove(Vector2 liftspeed)
    {
        savedLiftSpeed = liftspeed;
        boostSaveTimer = BoostSaveTime;
    }
}