using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.UI;

public struct RelativeX
{
    public float Mult = Engine.ViewWidth;

    public float X;
    public float Y;
    public float AbsValue => X * Mult;
    public RelativeX(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static implicit operator Vector2(RelativeX relative)
    {
        return new Vector2(relative.X * relative.Mult, relative.Y);
    }
}