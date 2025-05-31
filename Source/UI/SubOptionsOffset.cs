﻿using static Celeste.TextMenu;

namespace Celeste.Mod.LeniencyHelper.UI
{
    public class SubOptionsOffset : SubHeader
    {
        public int offset;
        public SubOptionsOffset(int offset) : base(string.Empty)
        {
            this.offset = offset;
        }
        public override float Height()
        {
            return offset;
        }
    }
}