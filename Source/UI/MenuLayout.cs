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
    public Rectangle VideoDestination;

    public float TweakScale;
    public float SubSettingScale;

    public DescriptionPos DescriptionPos;
    public float DescVerticalOffset;
    public Vector2 DescUnderVideo;
    
    
    public MenuLayout() { Update(); }
    public void Update()
    {
        VideoDestination = new Rectangle((int)(1920f - RightOffset + VideoOffsetX),
            (int)VideoPosY, (int)VideoSize.X, (int)VideoSize.Y);

        DescUnderVideo = new Vector2(1920f - RightOffset + VideoOffsetX, VideoPosY + DescVerticalOffset + VideoSize.Y);
    }
}