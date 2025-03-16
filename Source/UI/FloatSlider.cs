using System;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using static Celeste.TextMenu;
using Monocle;
using Celeste.Mod.LeniencyHelper.Components;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Tweaks;
using System.Diagnostics;

namespace Celeste.Mod.LeniencyHelper.UI
{
    public class FloatSlider : Option<float>
    {
        public float value = 0f;
        public bool isTimer = false;
        private float GetDefaultValue()
        {
            if (sessionVar is null || sessionVar == "") return 1f;
            else if (sessionVar.ToLower().Contains("buffertime")) return 0.08f;
            else
            {
                switch(sessionVar)
                {
                    case "DirectionalBufferTime": return 0.05f;
                    case "WallCorrectionTiming": return 0.05f;
                    case "FloorCorrectionTiming": return 0.1f;
                    case "wallLeniencyTiming": return 0.05f;
                    case "reversedFreezeTime": return 0.034f;
                    case "RefillCoyoteTime": return 0.05f;
                    case "RetainCbSpeedTime": return 0.1f;
                    case "bboostSaveTime": return 0.1f;
                    case "wallApproachTime": return 0.05f;
                    case "wallCoyoteTime": return 0.05f;
                    default: return 0.1f;
                }
            }
        }

        private float min, max;
        private int framesMin, framesMax;
        private int digits;

        public bool transitionIntoFrames = false;

        private float len = 0f;
        public string sessionVar;
        public FloatSlider(string label, float min, float max, float defaultValue, int digits, string sessionName) 
            : base(label)
        {
            value = (float)Math.Round(defaultValue, digits);
            this.max = max;
            this.min = min;
            framesMax = (int)Math.Floor(max * Engine.FPS);
            framesMin = (int)Math.Floor(min * Engine.FPS);
            this.digits = digits;
            sessionVar = sessionName;

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
            var s = LeniencyHelperModule.Settings;

            float actualBufferTime = value;
            if (sessionVar.Contains("Buffer"))
                actualBufferTime = (bool)LeniencyHelperModule.Settings.GetSetting("CustomBufferTime", "countBufferTimeInFrames")? value / Engine.FPS : value;
            
            switch (sessionVar)
            {
                case "JumpBufferTime":
                    s.buffers[Inputs.Jump] = value;
                    if (CustomBufferTime.newBuffers.ContainsKey(Inputs.Jump))
                        CustomBufferTime.newBuffers.Remove(Inputs.Jump);
                    CustomBufferTime.newBuffers.Add(Inputs.Jump, actualBufferTime);
                    break;

                case "DashBufferTime":
                    s.buffers[Inputs.Dash] = value;
                    if (CustomBufferTime.newBuffers.ContainsKey(Inputs.Dash))
                        CustomBufferTime.newBuffers.Remove(Inputs.Dash);
                    CustomBufferTime.newBuffers.Add(Inputs.Dash, actualBufferTime);
                    break;

                case "DemoBufferTime":
                    s.buffers[Inputs.Demo] = value;
                    if(CustomBufferTime.newBuffers.ContainsKey(Inputs.Demo))
                        CustomBufferTime.newBuffers.Remove(Inputs.Demo);
                    CustomBufferTime.newBuffers.Add(Inputs.Demo, actualBufferTime);
                    break;

                default:
                    LeniencyHelperModule.Settings.SetValue(sessionVar, value);
                    break;
            }
        }
        public override void ConfirmPressed()
        {
            value = transitionIntoFrames? (float)Math.Floor(GetDefaultValue() * Engine.FPS) : GetDefaultValue();
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