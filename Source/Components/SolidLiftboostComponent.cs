using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.SettingMaster;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Components;

public class SolidLiftboostComponent : Component
{
    public Collider drawRect;
    private float BoostSaveTime
    {
        get
        {
            return GetSetting<bool>("countSolidBoostSaveTimeInFrames") ?
                GetSetting<float>("bboostSaveTime") / Engine.FPS :
                GetSetting<float>("bboostSaveTime");
        }
    }
    public float boostSaveTimer = 0f;
    public Vector2 savedLiftSpeed = Vector2.Zero;
    public bool dontSetTimer = false;

    public SolidLiftboostComponent() : base(true, true) { Visible = true; }

    public void OnMove()
    {
        if (LeniencyHelperModule.Session.Tweaks["SolidBlockboostProtection"].Enabled)
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