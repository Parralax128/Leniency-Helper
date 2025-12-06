using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.UI;

public enum DescriptionPos
{
    AboveTweak,
    UnderPlayer
}

public struct MenuLayout
{
    public float LeftOffset;

    public float RightOffset;
    public float SubSettingOffset;

    public Vector2 VideoSize;
    public float VideoOffsetX;
    public float VideoPosY;

    public float TweakScale;
    public float SubSettingScale; 
    
    public Rectangle VideoDestination => new Rectangle((int)(Engine.ViewWidth - RightOffset + VideoOffsetX),
        (int)VideoPosY, (int)VideoSize.X, (int)VideoSize.Y);
    public MenuLayout() { }
}