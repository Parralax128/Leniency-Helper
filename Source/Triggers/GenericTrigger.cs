using System;
using Monocle;
using Celeste;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.LeniencyHelper.Tweaks;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using FMOD;
using IL.MonoMod;

namespace Celeste.Mod.LeniencyHelper.Triggers
{
    public class GenericTrigger : Trigger
    {
        public bool enabled;
        public string flag;
        public bool oneUse;
        public bool revertOnLeave;
        public bool wasEnabled;

        public GenericTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            enabled = data.Bool("Enabled", true);
            revertOnLeave = data.Bool("RevertOnLeave", true);
            flag = data.Attr("Flag", "");
            oneUse = data.Bool("OneUse", false);
        }
        public bool GetFlagActive()
        {
            return flag == "" ? true : SceneAs<Level>().Session.GetFlag(flag);
        }
        public virtual void UndoSettings() { }
        public virtual void GetOldSettings() { }
        public virtual void ApplySettings() { }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);

            if (revertOnLeave && !oneUse)
            {
                GetOldSettings();
            }

            ApplySettings();
            if (oneUse) RemoveSelf();
        }
        public override void OnStay(Player player)
        {
            base.OnStay(player);

            if(!oneUse) ApplySettings();
        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);

            if(!oneUse && revertOnLeave) UndoSettings();
        }

        public override void Update()
        {
            base.Update();
            Collidable = GetFlagActive();
        }
    }
}