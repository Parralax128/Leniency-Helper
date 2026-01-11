using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.UI.Items;

class HeightGap : TextMenu.Item
{
    public int offset;
    public HeightGap(int offset) : base() => this.offset = offset;
    public override float Height() => offset;
}