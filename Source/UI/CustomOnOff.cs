using System;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.SettingMaster;

namespace Celeste.Mod.LeniencyHelper.UI
{
    public class CustomOnOff : TextMenu.Option<bool>
    {
        public bool value;
        private string text => Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_ENUMVALUES_{(value?"ON":"OFF")}");
        public float len;
        private float Measure(bool on)
        {
            return ActiveFont.Measure(Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_ENUMVALUES_{(on ? "ON" : "OFF")}")).X;
        }

        public string settingName;
        public bool framesModeToggler;
        public CustomOnOff(string label, bool defaultValue, bool framesToggler, string settingName) : base(label)
        {
            value = defaultValue;
            this.settingName = settingName;
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

            if (value == GetDefaultSetting<bool>(settingName) || (value == false && GetDefaultSetting<bool>(settingName) == true)) lastDir = 1;
            else lastDir = -1;

            ValueWiggler.Start();

            value = GetDefaultSetting<bool>(settingName);
            if (OnValueChange != null) OnValueChange(value);
        }

        public override void Render(Vector2 position, bool highlighted)
        {
            Color inactiveColor = Color.DarkSlateGray;

            float alpha = Container.Alpha;
            Color strokeColor = Color.Black * (alpha * alpha * alpha);
            Color color = (Disabled ? inactiveColor : ((highlighted ? Container.HighlightColor : UnselectedColor) * alpha));
            ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);
            ActiveFont.DrawOutline(text, position + new Vector2(Container.Width - len * 0.5f + (float)lastDir * ValueWiggler.Value * 8f, 0f),
                new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);

            Vector2 vector = Vector2.UnitX * (highlighted ? ((float)Math.Sin(sine * 4f) * 4f) : 0f);


            Color leftArrowColor = (value ? color : (inactiveColor * alpha));
            Vector2 leftArrowPos = position + new Vector2(Container.Width - len + 40f + ((lastDir < 0) ?
                ((0f - ValueWiggler.Value) * 8f) : 0f), 0f) - (value ? vector : Vector2.Zero);
            ActiveFont.DrawOutline("<", leftArrowPos, new Vector2(0.5f, 0.5f), Vector2.One, leftArrowColor, 2f, strokeColor);

            Color color3 = (!value ? color : (inactiveColor * alpha));
            Vector2 position3 = position + new Vector2(Container.Width - 40f + ((lastDir > 0)?
                (ValueWiggler.Value * 8f) : 0f), 0f) + (!value ? vector : Vector2.Zero);
            ActiveFont.DrawOutline(">", position3, new Vector2(0.5f, 0.5f), Vector2.One, color3, 2f, strokeColor);
        }
    }
}
