using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Celeste;
using Monocle;
using Microsoft.Xna.Framework.Content;
using System.Runtime.CompilerServices;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.Triggers
{
    [CustomEntity("LeniencyHelper/ForceMoveTrigger")]
    public class ForceMoveTrigger : Trigger
    {
        public enum DirsX
        {
            Left,
            Right,
        }

        private DirsX forceMoveDir;
        private float ForceMoveTime;
        private bool countInFrames;

        private string flag;
        private bool oneUse;
        private bool onlyOnEnter;

        private bool flagActive => (flag == "" || (Scene as Level).Session.GetFlag(flag));
        public ForceMoveTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            forceMoveDir = data.Enum("Direction", DirsX.Right);
            ForceMoveTime = data.Float("ForceMoveTime", 0.1f);
            countInFrames = data.Bool("CountInFrames", false);

            flag = data.Attr("Flag", "");
            oneUse = data.Bool("OneUse", false);
            onlyOnEnter = data.Bool("OnlyOnEnter", true);
        }

        private static int DirToInt(DirsX dir)
        {
            if (dir == DirsX.Left) return -1;
            else return 1;
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);

            if (!flagActive) return;

            player.forceMoveX = DirToInt(forceMoveDir);
            player.forceMoveXTimer = countInFrames ? ForceMoveTime/Engine.FPS : ForceMoveTime;

            if(oneUse) RemoveSelf();
        }
        public override void OnStay(Player player)
        {
            base.OnStay(player);

            if(!onlyOnEnter && flagActive)
            {
                player.forceMoveX = DirToInt(forceMoveDir);
                player.forceMoveXTimer = countInFrames ? ForceMoveTime*Engine.DeltaTime : ForceMoveTime;
            }
        }
    }
}