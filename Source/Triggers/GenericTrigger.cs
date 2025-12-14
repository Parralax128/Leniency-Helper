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
            
        protected bool Enabled;
        protected bool OneUse;
        protected bool RevertOnLeave;
        string flag;
        bool applyOnStay;

        public GenericTrigger(EntityData data, Vector2 offset, bool applyOnStay = false) : base(data, offset)
        {
            Enabled = data.Bool("Enabled", true);
            RevertOnLeave = data.Bool("RevertOnLeave", true);
            flag = data.Attr("Flag", "");
            OneUse = data.Bool("OneUse", false);
            this.applyOnStay = applyOnStay;
        }
        protected bool FlagActive => flag == "" || SceneAs<Level>().Session.GetFlag(flag);
        
        protected virtual void Undo(Player player) { }
        protected virtual void SaveData() { }
        protected virtual void Apply(Player player) { }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);

            if (RevertOnLeave) SaveData();
            Apply(player);
        }
        public override void OnStay(Player player)
        {
            base.OnStay(player);

            if (applyOnStay) Apply(player);
        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);

            if (RevertOnLeave) Undo(player);
            if (OneUse) RemoveSelf();
        }

        public override void Update()
        {
            base.Update();
            Collidable = FlagActive;
        }
    }
}