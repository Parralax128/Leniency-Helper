using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.Triggers
{
    public abstract class GenericTrigger : Trigger
    {
        public static List<string>StringToList(string str)
        {
            List<string> result = new List<string>();

            string currentWord = "";
            for(int index=0; index < str.Length; index++)
            {
                if (str[index] == ',')
                {
                    result.Add(currentWord);
                    currentWord = "";
                }
                else if(index == str.Length - 1)
                {
                    currentWord += str[index];
                    result.Add(currentWord);
                    currentWord = "";
                }
                else if (str[index] != ' ')
                {
                    currentWord += str[index];
                }
            }

            return result;
        }

        public bool enabled;
        string flag;
        public bool oneUse;
        public bool revertOnLeave;

        bool applyOnStay;

        public GenericTrigger(EntityData data, Vector2 offset, bool applyOnStay = false) : base(data, offset)
        {
            enabled = data.Bool("Enabled", true);
            revertOnLeave = data.Bool("RevertOnLeave", true);
            flag = data.Attr("Flag", "");
            oneUse = data.Bool("OneUse", false);
            this.applyOnStay = applyOnStay;
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
        }
        public override void OnStay(Player player)
        {
            base.OnStay(player);

            if (!oneUse && applyOnStay) ApplySettings();
        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);

            if (revertOnLeave) UndoSettings();
            if (oneUse) RemoveSelf();
        }

        public override void Update()
        {
            base.Update();
            Collidable = GetFlagActive();
        }
    }
}