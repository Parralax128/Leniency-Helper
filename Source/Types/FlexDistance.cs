namespace Celeste.Mod.LeniencyHelper;
static class FlexDistance
{
    public enum Modes
    { 
        Static,
        Dynamic
    }

    public static int Get(Modes mode, int staticDist, Time time, float Speed) => mode == Modes.Static ? staticDist : (int)(time * Speed);
}