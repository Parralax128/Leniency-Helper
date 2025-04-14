using System;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using static Celeste.Mod.LeniencyHelper.SettingMaster;
using static Celeste.TextMenu;
using Monocle;
using Celeste.Mod.LeniencyHelper.Tweaks;

namespace Celeste.Mod.LeniencyHelper.UI
{
    public class FloatSlider : Option<float>
    {
        public float value = 0f;
        public bool isTimer = false;

        private float min, max;
        private int framesMin, framesMax;
        private int digits;

        public bool transitionIntoFrames = false;

        private float len = 0f;
        public string settingName;
        public FloatSlider(string label, float min, float max, float defaultValue, int digits, string sessionName) 
            : base(label)
        {
            value = (float)Math.Round(defaultValue, digits);

            this.max = (float)Math.Round(max, digits);
            if (this.max > max) this.max = (float)Math.Round(max - (float)Math.Pow(10, -digits), digits);

            this.min = min;
            framesMax = (int)Math.Floor(max * Engine.FPS);
            framesMin = (int)Math.Floor(min * Engine.FPS);
            this.digits = digits;
            settingName = sessionName;

            float maxLen = 0;
            for (float c = min; c < max; 
                c = (float)Math.Round(Calc.Approach(c, max, (float)Math.Pow(10, -digits)), digits))
            {
                float currentLen = ActiveFont.Measure(c.ToString()).X;
                if (maxLen < currentLen) maxLen = currentLen;
            }
            len = maxLen + 120f;
        }
        public void SwapMode(bool toFrames)
        {
            if (toFrames == transitionIntoFrames) return;

            if (toFrames)
            {
                value = (float)Math.Floor(value * Engine.FPS);
            }
            else value = (float)Math.Round((float)value / Engine.FPS, digits);                

            transitionIntoFrames = toFrames;
        }
        public override void LeftPressed()
        {
            Audio.Play("event:/ui/main/button_toggle_off");
            lastDir = -1;

            if (transitionIntoFrames) value = (float)Math.Floor(Calc.Approach(value, (float)framesMin, 1));
            else value = (float)Math.Round(Calc.Approach(value, min, (float)Math.Pow(10, -digits)), digits);

            ValueWiggler.Start();

            ChangedValue();
        }
        public override void RightPressed()
        {
            Audio.Play("event:/ui/main/button_toggle_on");
            lastDir = 1;

            if (transitionIntoFrames) value = (float)Math.Floor(Calc.Approach(value, (float)framesMax, 1));
            else value = (float)Math.Round(Calc.Approach(value, max, (float)Math.Pow(10, -digits)), digits);

            ValueWiggler.Start();

            ChangedValue();
        }
        public void ChangedValue()
        {
            SetPlayerSetting(settingName, value);
            if (OnValueChange != null) OnValueChange(value);
        }
        
        public override void ConfirmPressed()
        {
            value = transitionIntoFrames ? (float)Math.Floor(GetDefaultSetting<float>(settingName) * Engine.FPS) : GetDefaultSetting<float>(settingName);
            ChangedValue();
        }
        public override float RightWidth() { return len; }
        public override float LeftWidth() { return len; }

        public override void Render(Vector2 position, bool highlighted)
        {
            float alpha = Container.Alpha;
            Color strokeColor = Color.Black * (alpha * alpha * alpha);
            Color color = (Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : UnselectedColor) * alpha));
            ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);
            
            float num = RightWidth();
            ActiveFont.DrawOutline(value.ToString(), position + new Vector2(Container.Width - num * 0.5f + (float)lastDir * ValueWiggler.Value * 8f, 0f),
                new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);

            Vector2 vector = Vector2.UnitX * (highlighted ? ((float)Math.Sin(sine * 4f) * 4f) : 0f);

            bool flag = value > min;
            if (transitionIntoFrames) flag = value > (float)framesMin;

            Color color2 = (flag ? color : (Color.DarkSlateGray * alpha));
            Vector2 position2 = position + new Vector2(Container.Width - num + 40f + ((lastDir < 0) ?
                ((0f - ValueWiggler.Value) * 8f) : 0f), 0f) - (flag ? vector : Vector2.Zero);
            ActiveFont.DrawOutline("<", position2, new Vector2(0.5f, 0.5f), Vector2.One, color2, 2f, strokeColor);
                
            bool flag2 = value < max;
            if (transitionIntoFrames) flag2 = value < (float)framesMax;

            Color color3 = (flag2 ? color : (Color.DarkSlateGray * alpha));
            Vector2 position3 = position + new Vector2(Container.Width - 40f + ((lastDir > 0) ? (ValueWiggler.Value * 8f) : 0f), 0f) + (flag2 ? vector : Vector2.Zero);
            ActiveFont.DrawOutline(">", position3, new Vector2(0.5f, 0.5f), Vector2.One, color3, 2f, strokeColor);
        }
    }
}