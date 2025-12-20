namespace Celeste.Mod.LeniencyHelper.Components;


class DisableAirMovementComponent : PersistentComponent<Player>
{
    public bool Activated = false;
    public DisableAirMovementComponent() { Debug.Warn("DISALBE AIR MOEVEMTN COMPONENT!!!"); }
}