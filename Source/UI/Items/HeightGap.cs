using static Celeste.TextMenu;

namespace Celeste.Mod.LeniencyHelper.UI.Items
{
    class HeightGap : SubHeader
    {
        public int offset;
        public HeightGap(int offset) : base(string.Empty)
        {
            this.offset = offset;
        }
        public override float Height()
        {
            return offset;
        }
    }
}