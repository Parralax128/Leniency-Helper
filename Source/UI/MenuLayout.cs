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
    private float _leftOffset = 0f;
    public float LeftOffset 
    {
        get => _leftOffset * Engine.ViewWidth;
        set => _leftOffset = value;
    }

    private float _rightOffset = 0f;
    public float RightOffset
    {
        get => _rightOffset * Engine.ViewWidth;
        set => _rightOffset = value;
    }

    private float _subSettingOffset = 0f;   
    public float SubSettingOffset
    {
        get => _subSettingOffset * Engine.ViewWidth;
        set => _subSettingOffset = value;
    }

    private float _vidSize = 0.4f;
    public Vector2 VideoSize
    {
        get => new Vector2(Engine.ViewWidth, Engine.ViewHeight) * _vidSize;
        set => _vidSize = value.X;
    }
    private float _vidOffsetX;
    public float VideoOffsetX
    {
        get => Engine.ViewWidth * _vidOffsetX;
        set => _vidOffsetX = value;
    }
    
    private float _vidPosY;
    public float VideoPosY
    {
        get => Engine.ViewHeight * _vidPosY;
        set => _vidOffsetX = value;
    }
    

    public Rectangle VideoDestination => new Rectangle((int)(Engine.ViewWidth - RightOffset + VideoOffsetX),
        (int)VideoPosY, (int)VideoSize.X, (int)VideoSize.Y);
    public MenuLayout() { }
}