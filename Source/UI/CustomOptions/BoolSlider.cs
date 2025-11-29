using Microsoft.Xna.Framework;
using System;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using static Celeste.Mod.LeniencyHelper.SettingMaster;

namespace Celeste.Mod.LeniencyHelper.UI.CustomOptions
{
    public class BoolSlider : TweakSetting<bool>
    {
        private string text => Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_ENUMVALUES_{(value ? "ON" : "OFF")}");
        public float len;
        private float Measure(bool on)
        {
            return ActiveFont.Measure(Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_ENUMVALUES_{(on ? "ON" : "OFF")}")).X;
        }

        public bool framesModeToggler;
        public BoolSlider(bool framesToggler, Tweak tweak, string settingName)
            : base(tweak, settingName)
        {
            framesModeToggler = framesToggler;
            len = 120f + Math.Max(Measure(true), Measure(false));

            OnValueChange += (value) => { SetPlayerSetting(this.settingName, value); };
        }

        public override void RightPressed()
        {
            if (!value)
            {
                lastDir = 1;

                value = true;
                if (OnValueChange != null) OnValueChange(value);
            }
            Audio.Play("event:/ui/main/button_toggle_on");
            ValueWiggler.Start();
        }
        public override void LeftPressed()
        {
            if (value)
            {
                lastDir = -1;

                value = false;
                if (OnValueChange != null) OnValueChange(value);
            }
            Audio.Play("event:/ui/main/button_toggle_off");
            ValueWiggler.Start();
        }
        public override void ConfirmPressed()
        {
            if (Index == 0)
            {
                Audio.Play("event:/ui/main/button_toggle_on");
            }
            else
            {
                Audio.Play("event:/ui/main/button_toggle_off");
            }

            if (value == GetDefaultSetting<bool>(settingName) || value == false && GetDefaultSetting<bool>(settingName) == true) lastDir = 1;
            else lastDir = -1;

            ValueWiggler.Start();

            value = GetDefaultSetting<bool>(settingName);
            if (OnValueChange != null) OnValueChange(value);
        }
        public override float RightWidth()
        {
            string True = Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_ENUMVALUES_ON"), False = Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_ENUMVALUES_OFF");
            return Math.Max(ActiveFont.Measure(True).X, ActiveFont.Measure(False).X);
        }

        public override void Render(Vector2 position, bool selected)
        {
            BaseRender(text, position, selected, value == true, value == false);
        }
    }
}
