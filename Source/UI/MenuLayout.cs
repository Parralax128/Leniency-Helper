using Monocle;

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

    public MenuLayout() { }
}