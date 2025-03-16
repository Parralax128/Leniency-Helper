using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Tweaks;
using Microsoft.Xna.Framework;
using YamlDotNet.Core.Tokens;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using static Celeste.TextMenu;

namespace Celeste.Mod.LeniencyHelper.UI
{
    public class SpecialSlider : TextMenu.Option<int>
    {
        public TextMenu menu;

        public List<Item> subOptions = new List<Item>();
        public bool addedDetailOptions = false;

        private SubOptionsSubheader beforeSubOptions;
        private SubOptionsSubheader afterSubOptions;

        public string tweakName;

        private static Color PlayerValueColor = new Color(218, 165, 32);
        private static Color MapValueColor = new Color(0, 191, 255);
        private static Color BothInactiveColor = Color.White;
        public enum SliderValues
        {
            Map,
            On,
            Off
        }
        private static string GetDialogEnumValue(SliderValues enumValue)
        {
            return Dialog.Clean($"MODOPTIONS_LENIENCYHELPER_ENUMVALUES_{enumValue.ToString().ToUpper()}");
        }
        public static int GetIndexFromTweakName(string tweakName)
        {
            if (LeniencyHelperModule.Settings.TweaksByPlayer[tweakName] is not null)
                return (LeniencyHelperModule.Settings.TweaksByPlayer[tweakName] == true ? 1 : 2);
            else return 0;
        }
        public SpecialSlider(string label, string tweakName, int defaultIndex) : base(label)
        {
            this.tweakName = tweakName;

            Add(GetDialogEnumValue(SliderValues.Map), 0);
            Add(GetDialogEnumValue(SliderValues.On), 1);
            Add(GetDialogEnumValue(SliderValues.Off), 2);

            this.Index = this.PreviousIndex = defaultIndex;

            AddSubOptions();

            if (subOptions.Count > 0)
            {
                beforeSubOptions = new SubOptionsSubheader(8);
                afterSubOptions = new SubOptionsSubheader(16);
            }
        }

        private void AddSubOptions()
        {
            switch (tweakName)
            {
                case "BufferableClimbtrigger":
                    SetSubOption(ToDialogID("Disable instant upward climb-activation"), "onlyOnClimbjumps", typeof(bool));
                    break;

                case "BufferableExtends":
                    SetSubOption(ToDialogID("ForceWaitForRefill"), "forceWaitForRefill", typeof(bool));                    
                    break;

                case "ConsistentDashOnDBlockExit":
                    SetSubOption(ToDialogID("ResetDashCD"), "resetDashCDonLeave", typeof(bool));
                    break;

                case "CornerWaveLeniency":
                    SetSubOption(ToDialogID("AllowSpikedFloor"), "allowSpikedFloor", typeof(bool));
                    break;

                case "CustomBufferTime":
                    SetSubOption(ToDialogID("CountInFrames"), "countBufferTimeInFrames", typeof(bool));
                    SetupCustomBufferOptions();
                    break;

                case "DirectionalReleaseProtection":
                    SetSubOption(ToDialogID("CountInFrames"), "CountProtectionTimeInFrames", typeof(bool));
                    SetSubOption(ToDialogID("ProtectedJumpDirection"), "jumpDir", typeof(Dirs));
                    SetSubOption(ToDialogID("ProtectedDashDirection"), "dashDir", typeof(Dirs));
                    SetSubOption(ToDialogID("ProtectedAfterReleaseTime"), "DirectionalBufferTime", typeof(float));
                    break;

                case "DynamicCornerCorrection":
                    SetSubOption(ToDialogID("CountInFrames"), "ccorectionTimingInFrames", typeof(bool));
                    SetSubOption(ToDialogID("WallCorrectionTiming"), "WallCorrectionTiming", typeof(float));
                    SetSubOption(ToDialogID("FloorCorrectionTiming"), "FloorCorrectionTiming", typeof(float));                    
                    break;

                case "DynamicWallLeniency":
                    SetSubOption(ToDialogID("CountInFrames"), "countWallTimingInFrames", typeof(bool));
                    SetSubOption(ToDialogID("wallLeniencyTiming"), "wallLeniencyTiming", typeof(float));
                    break;

                case "ExtendBufferOnFreezeAndPickup":
                    SetSubOption(ToDialogID("ExtendBuffersOnPickup"), "ExtendBufferOnPickup", typeof(bool));
                    SetSubOption(ToDialogID("ExtendBuffersOnFreeze"), "ExtendBufferOnFreeze", typeof(bool));
                    break;

                case "FrozenReverses":
                    SetSubOption(ToDialogID("CountInFrames"), "countReversedInFrames", typeof(bool));
                    SetSubOption(ToDialogID("FreezeTime"), "reversedFreezeTime", typeof(float));
                    break;

                case "IceWallIncreaseWallLeniency":
                    SetSubOption(ToDialogID("AdditionalWallLeniency"), "iceWJLeniency", typeof(int));
                    break;

                case "RefillDashInCoyote":
                    SetSubOption(ToDialogID("CountInFrames"), "CountRefillCoyoteTimeInFrames", typeof(bool));
                        SetSubOption(ToDialogID("CoyoteRefillTime"), "RefillCoyoteTime", typeof(float));
                    break;

                case "RetainSpeedCornerboost":
                    SetSubOption(ToDialogID("CountInFrames"), "countRetainTimeInFrames", typeof(bool));
                    SetSubOption(ToDialogID("RetainTime"), "RetainCbSpeedTime", typeof(float));
                    break;

                case "SolidBlockboostProtection":
                    SetSubOption(ToDialogID("CountInFrames"), "countSolidBoostSaveTimeInFrames", typeof(bool));
                    SetSubOption(ToDialogID("ProtectionTime"), "bboostSaveTime", typeof(float));
                    break;

                case "WallAttraction":
                    SetSubOption(ToDialogID("CountInFrames"), "countAttractionTimeInFrames", typeof(bool));
                    SetSubOption(ToDialogID("ApproachTime"), "wallApproachTime", typeof(float));
                    break;

                case "WallCoyoteFrames":
                    SetSubOption(ToDialogID("CountInFrames"), "countWallCoyoteTimeInFrames", typeof(bool));
                    SetSubOption(ToDialogID("WallCoyoteTime"), "wallCoyoteTime", typeof(float));
                    break;
            }

            foreach (CustomOnOff toggler in subOptions.FindAll(i => i.GetType() == typeof(CustomOnOff)).ToList())
            {
                if (toggler.framesModeToggler)
                {
                    toggler.OnValueChange += (value) =>
                    {
                        foreach (FloatSlider slider in subOptions.FindAll(slider => slider.GetType() == typeof(FloatSlider)
                            && (slider as FloatSlider).isTimer).ToList())
                        {
                            slider.SwapMode(value);
                            slider.ChangedValue();
                        }
                    };

                    foreach (FloatSlider slider in subOptions.FindAll(slider =>
                        slider.GetType() == typeof(FloatSlider)).ToList())
                    {
                        slider.transitionIntoFrames = toggler.value;
                    }

                    break;
                }
            }
        }
        private void SetSubOption(string label, string nameInSettings, Type type)
        {
            var settings = LeniencyHelperModule.Settings;

            Item newOption;

            if (type == typeof(bool))
            {
                newOption = new CustomOnOff(label, (bool)settings.GetSettingByIndexPriority(Index, nameInSettings),
                    nameInSettings.ToLower().Contains("inframes"), nameInSettings);

                this.OnValueChange += (value) =>
                {
                    (newOption as CustomOnOff).value = (bool)settings.GetSettingByIndexPriority(Index, nameInSettings);
                };
            }
            else if (type == typeof(float))
            {
                newOption = new FloatSlider(label, 0f, GetMaxFromName(nameInSettings),
                    (float)settings.GetSettingByIndexPriority(Index,  nameInSettings), 2, nameInSettings);

                (newOption as FloatSlider).isTimer = GetIsTimer(nameInSettings);

                this.OnValueChange += (value) =>
                {
                    (newOption as FloatSlider).value = 
                    (float)settings.GetSettingByIndexPriority(Index, nameInSettings);
                };

            }
            else if (type == typeof(int))
            {
                newOption = new IntSlider(label, 0, (int)GetMaxFromName(nameInSettings),
                    (int)settings.GetSettingByIndexPriority(Index, nameInSettings), nameInSettings);

                this.OnValueChange += (value) =>
                {
                    (newOption as IntSlider).value = 
                    (int)settings.GetSettingByIndexPriority(Index, nameInSettings);
                };
            }
            else if(type == typeof(Dirs))
            {
                Dictionary<string, int> parsedEnum = new Dictionary<string, int>
                {
                    { "Up",    0 },
                    { "Down",  1 },
                    { "Left",  2 },
                    { "Right", 3 },
                    { "All",   4 },
                    { "None",  5 },
                };

                newOption = new EnumSlider(label, parsedEnum);
                Dirs defaultIndex = (Dirs)settings.GetSettingByIndexPriority(Index, nameInSettings);

                foreach(string key in parsedEnum.Keys)
                {
                    if (key == defaultIndex.ToString())
                    {
                        (newOption as EnumSlider).Index = parsedEnum[key];
                        break;
                    }
                }

                (newOption as Option<int>).OnValueChange += (value) =>
                {
                    settings.SetValue(nameInSettings, IndexToDir(value));
                };

                this.OnValueChange += (value) =>
                {
                    foreach (string key in parsedEnum.Keys)
                    {
                        if (key == settings.GetSettingByIndexPriority(Index, nameInSettings).ToString())
                        {
                            (newOption as EnumSlider).Index = parsedEnum[key];
                            break;
                        }
                    }
                };
            }
            else return;

            subOptions.Add(newOption);
        }
        public float GetMaxFromName(string name)
        {
            switch (name)
            {
                case "DirectionalBufferTime": return 0.5f;
                case "reversedFreezeTime": return 0.25f;
                case "iceWJLeniency": return 16f;
                case "RefillCoyoteTime": return 0.085f;
                case "wallApproachTime": return 0.25f;
                case "wallCoyoteTime": return 0.25f;
            }
            if (name.Contains("Timing")) return 0.25f;

            return 0.5f;
        }

        public string ToDialogID(string str)
        {
            return ("     " + Dialog.Clean("MODOPTIONS_LENIENCYHELPER_SETTINGS_" + str.ToUpper()));
        }
        private Dirs IndexToDir(int index)
        {
            switch (index)
            {
                case 0: return Dirs.Up;
                case 1: return Dirs.Down;
                case 2: return Dirs.Left;
                case 3: return Dirs.Right;
                case 4: return Dirs.All;
                default: return Dirs.None;
            }
        }
        private bool GetIsTimer(string paramName)
        {
            return paramName.ToLower().Contains("time") || paramName.ToLower().Contains("timing");
        }
        private void SetupCustomBufferOptions()
        {
            var settings = LeniencyHelperModule.Settings;

            for(int c=2; c>=0; c--)
            {
                Inputs input = CustomBufferTime.VanillaInputs[c];

                FloatSlider newSlider = new FloatSlider(ToDialogID($"{input}BufferTime"), 0f, 1f,
                    GetBufferTimeBySettingPriority(Index, input), 2, $"{input}BufferTime");
                newSlider.isTimer = true;

                this.OnValueChange += (value) =>
                { newSlider.value = GetBufferTimeBySettingPriority(Index, input); };

                subOptions.Add(newSlider);
            }
            
            this.OnValueChange += (index) => 
            {
                CustomBufferTime.UpdateCustomBuffers(index);
            };
        }
        private float GetBufferTimeBySettingPriority(int index, Inputs input)
        {
            if (index == 0) return LeniencyHelperModule.Session.mapBuffers[input];
            else return LeniencyHelperModule.Settings.buffers[input];
        }

        public override void ConfirmPressed()
        {
            if (!LeniencyHelperModule.Session.TweaksEnabled[tweakName])
            {
                CloseDetailOptions();
            }
            else
            {
                OpenDetailOptions();
            }
        }

        public void OpenDetailOptions()
        {
            if (addedDetailOptions || subOptions.Count <= 0) return;

            //disabling possibility to change options if "Map" is selected
            if (Index == 0) foreach (Item item in subOptions) item.Disabled = true;
            else foreach (Item item in subOptions) item.Disabled = false;

            int optionsIndex = menu.Items.FindIndex(item => item.GetType() == typeof(SpecialSlider) && ((SpecialSlider)item).Label == Label);
            optionsIndex++;

            menu.Insert(optionsIndex-1, beforeSubOptions);

            bool foundSkip = false;
            foreach (Item option in subOptions)
            {
                if (tweakName == "CustomBufferTime" && option.SearchLabel() == "     "+Dialog.Clean("MODOPTIONS_LENIENCYHELPER_SETTINGS_COUNTINFRAMES")
                    && Index == 0)
                {
                    foundSkip = true;
                    continue;
                }
                    
                menu.Insert(optionsIndex + 1, option);
                menu.Selection = optionsIndex + 1;

                if (subOptions.Count == 1) MenuButtonManager.InSingleItemSuboptionsMenu = true;
                else
                {
                    MenuButtonManager.InSingleItemSuboptionsMenu = false;
                    option.OnLeave += () =>
                    {
                        LoopMenuOnLeave(optionsIndex + 1, optionsIndex + subOptions.Count);
                    };
                }
            }            
            if (Index == 0) menu.Selection = optionsIndex;

            if (foundSkip) optionsIndex--;
            menu.Insert(optionsIndex + 1 + subOptions.Count, afterSubOptions);

            MenuButtonManager.InSubOptionMode = true;
            addedDetailOptions = true;
        }
        private void LoopMenuOnLeave(int minIndex, int maxIndex)
        {
            if (menu.Selection > maxIndex) menu.Selection = minIndex;
            else if(menu.Selection <  minIndex) menu.Selection = maxIndex;
        }
        public override void RightPressed()
        {
            base.RightPressed();
            if(Index == 1 && addedDetailOptions) //if switched from "Map" to "On" - updating suboptions via reopening
            {
                CloseDetailOptions();
                OpenDetailOptions();
            }
        }
        public void CloseDetailOptions()
        {
            if (!addedDetailOptions || subOptions.Count <= 0) return;

            foreach (Item item in subOptions)
            {
                menu.Remove(item);
            }
            menu.Remove(beforeSubOptions);
            menu.Remove(afterSubOptions);

            MenuButtonManager.InSubOptionMode = false;
            MenuButtonManager.InSingleItemSuboptionsMenu = false;
            addedDetailOptions = false;

            menu.Selection = menu.Items.FindIndex(item => item.Equals(this));
        }

        
        private Color GetColorOnEnable()
        {
            if (LeniencyHelperModule.Settings.TweaksByPlayer[tweakName] is not null) return PlayerValueColor;
            else
            {
                if (LeniencyHelperModule.Session.TweaksByMap[tweakName])
                {
                    return MapValueColor;
                }
                else return BothInactiveColor;
            }
        }
        public override void Render(Vector2 position, bool highlighted)
        {
            float alpha = Container.Alpha;
            Color strokeColor = Color.Black * (alpha * alpha * alpha);
            Color color = (Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : UnselectedColor) * alpha));

            if(!Disabled && !highlighted)
            {
                color = GetColorOnEnable();
            }

            ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);
            if (Values.Count > 0)
            {
                float num = RightWidth();

                ActiveFont.DrawOutline(Values[Index].Item1, position + 
                    new Vector2(Container.Width - num * 0.5f + (float)lastDir * ValueWiggler.Value * 8f, 0f),
                    new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);

                Vector2 vector = Vector2.UnitX * (highlighted ? ((float)Math.Sin(sine * 4f) * 4f) : 0f);
                bool flag = Index > 0;
                Color color2 = (flag ? color : (Color.DarkSlateGray * alpha));

                Vector2 position2 = position + 
                    new Vector2(Container.Width - num + 40f + ((lastDir < 0) ? ((0f - ValueWiggler.Value) * 8f) : 0f), 0f)
                    - (flag ? vector : Vector2.Zero);

                ActiveFont.DrawOutline("<", position2, new Vector2(0.5f, 0.5f), Vector2.One, color2, 2f, strokeColor);


                bool flag2 = Index < Values.Count - 1;
                Color color3 = (flag2 ? color : (Color.DarkSlateGray * alpha));
                Vector2 position3 = position + new Vector2(Container.Width - 40f + 
                    ((lastDir > 0) ? (ValueWiggler.Value * 8f) : 0f), 0f) + (flag2 ? vector : Vector2.Zero);

                ActiveFont.DrawOutline(">", position3, new Vector2(0.5f, 0.5f), Vector2.One, color3, 2f, strokeColor);
            }
        }
    }
}