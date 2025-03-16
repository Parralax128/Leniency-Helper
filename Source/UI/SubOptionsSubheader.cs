using static Celeste.TextMenu;

namespace Celeste.Mod.LeniencyHelper.UI
{
    public class SubOptionsSubheader : SubHeader
    {
        public int offset;
        public SubOptionsSubheader(int offset) : base(string.Empty)
        {
            this.offset = offset;
        }
        public override float Height()
        {
            return (float)offset;
        }
    }
}