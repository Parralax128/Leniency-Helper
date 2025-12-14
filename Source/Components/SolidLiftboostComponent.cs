using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Components;

class SolidLiftboostComponent : TweakComponent<Platform, Tweaks.SolidBlockboostProtection>
{
    public Timer BoostSaveTimer = new();
    public Vector2 SavedLiftSpeed = Vector2.Zero;
    public bool DontSetTimer = false;

    public void OnMove()
    {
        if (!TweakEnabled) return;
        
        if(Entity.LiftSpeed != Vector2.Zero)
        {
            SavedLiftSpeed = Entity.LiftSpeed;
            BoostSaveTimer.Launch(GetSetting<Time>()); 
        }
    }
    public void OnSidewaysMove(Vector2 liftspeed)
    {
        SavedLiftSpeed = liftspeed;
        BoostSaveTimer.Launch(GetSetting<Time>());
    }
}