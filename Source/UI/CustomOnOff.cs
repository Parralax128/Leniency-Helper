using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using YamlDotNet.Core.Tokens;


namespace Celeste.Mod.LeniencyHelper.UI
{
    public class CustomOnOff : TextMenu.Option<bool>
    {
        public bool value;
        private string text => Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_ENUMVALUES_{(value?"ON":"OFF")}");
        public float len => 120f + ActiveFont.Measure(text).X;
        private string sessionVar;
        public bool framesModeToggler;
        
        private bool GetDefaultValue()
        {
            return true;

        }
        public CustomOnOff(string label, bool defaultValue, bool framesToggler, string sessionName) : base(label)
        {
            value = defaultValue;
            sessionVar = sessionName;
            framesModeToggler = framesToggler;

            this.OnValueChange += (value) =>
            {
                LeniencyHelperModule.Settings.SetValue(sessionVar, value);
            };
        }

        public override void RightPressed()
        {
            if (!value)
            {
                Audio.Play("event:/ui/main/button_toggle_on");
                lastDir = 1;
                ValueWiggler.Start();
                
                value = true;
                if (OnValueChange != null) OnValueChange(value);
            }
        }
        public override void LeftPressed()
        {
            if (value)
            {
                Audio.Play("event:/ui/main/button_toggle_off");
                lastDir = -1;
                ValueWiggler.Start();
                
                value = false;
                if (OnValueChange != null) OnValueChange(value);
            }
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

            if (value == GetDefaultValue() || (value == false && GetDefaultValue() == true)) lastDir = 1;
            else lastDir = -1;

            ValueWiggler.Start();
            
            value = GetDefaultValue();
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
