using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.LeniencyHelper.UI.Items;

class Description : TextMenu.Item
{
    class FancyText
    {
        struct TextFragment
        {
            public string Text;
            public Color Color;
        }

        static readonly Dictionary<string, Color> TagColors = new()
        {
            { "ins", Color.SlateGray }
        };

        public string String;
        static readonly float BaseScale = 0.8f;
        public Func<float> ScaleGetter { get; set; } = null;
        float EffectiveScale => ScaleGetter?.Invoke() ?? BaseScale;

        List<TextFragment> parsedFragments = new();
        List<List<TextFragment>> wrappedLines = new(); // Final layout: Lines contain Fragments

        public float LineWidth { get; set; }
        public float Height => wrappedLines.Count * ActiveFont.LineHeight * EffectiveScale;

        public FancyText(string text)
        {
            String = text;
            LineWidth = 1920f - TweakMenuManager.Layout.LeftOffset - TweakMenuManager.Layout.RightOffset;

            ParseFragments();
            WrapLines();
        }
        public FancyText(string text, Func<float> scaleGetter) : this(text)
        {
            ScaleGetter = scaleGetter;
        }

        void ParseFragments()
        {
            if (string.IsNullOrEmpty(String))
            {
                parsedFragments = new() { new TextFragment() { Text = "", Color = Color.White } };
                return;
            }

            parsedFragments.Clear();
            Color currentColor = Color.DarkSlateGray;
            var currentText = new System.Text.StringBuilder();
            bool insideTag = false;
            var tagBuffer = new System.Text.StringBuilder();

            for (int i = 0; i < String.Length; i++)
            {
                char c = String[i];

                if (c == '<' && !insideTag)
                {
                    if (currentText.Length > 0)
                    {
                        parsedFragments.Add(new TextFragment
                        {
                            Text = currentText.ToString(),
                            Color = currentColor
                        });
                        currentText.Clear();
                    }
                    insideTag = true;
                    tagBuffer.Clear();
                    continue;
                }

                if (c == '>' && insideTag)
                {
                    insideTag = false;
                    string tag = tagBuffer.ToString();

                    if (tag.StartsWith("/"))
                        currentColor = Color.DarkSlateGray;

                    else if (TagColors.TryGetValue(tag, out Color newColor))
                        currentColor = newColor;

                    continue;
                }

                if (insideTag) tagBuffer.Append(c);
                else currentText.Append(c);
            }

            if (currentText.Length > 0)
            {
                parsedFragments.Add(new TextFragment
                {
                    Text = currentText.ToString(),
                    Color = currentColor
                });
            }
        }

        void WrapLines()
        {
            wrappedLines.Clear();
            List<TextFragment> currentLine = new();
            float currentWidth = 0f;

            if(string.IsNullOrEmpty(String))
            {
                wrappedLines = new() { parsedFragments };
            }

            foreach (var fragment in parsedFragments)
            {
                string[] words = fragment.Text.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words)
                {
                    if (word == "\n" || word.Contains("\n"))
                    {
                        if (currentLine.Count > 0) wrappedLines.Add(currentLine);
                        currentLine.Clear();
                        currentWidth = 0f;
                        continue;
                    }

                    float wordWidth = ActiveFont.Measure(word + ' ').X * EffectiveScale;

                    if (currentWidth + wordWidth > LineWidth && currentLine.Count > 0)
                    {
                        wrappedLines.Add(new List<TextFragment>(currentLine));
                        currentLine.Clear();
                        currentWidth = 0f;
                    }

                    currentLine.Add(new TextFragment
                    {
                        Text = word,
                        Color = fragment.Color
                    });
                    currentWidth += wordWidth;
                }
            }

            if (currentLine.Count > 0)
                wrappedLines.Add(currentLine);
        }


        public void Render(Vector2 position)
        {
            position.Y -= Height / 2f;

            float scale;
            float lineHeight = ActiveFont.LineHeight * (scale = EffectiveScale);
            float y = position.Y;

            foreach (List<TextFragment> line in wrappedLines)
            {
                float x = position.X;
                foreach (TextFragment fragment in line)
                {
                    ActiveFont.DrawOutline(
                        fragment.Text + ' ',
                        new Vector2(x, y),
                        Vector2.Zero,
                        Vector2.One * scale,
                        fragment.Color,
                        2f,
                        Color.Black
                    );
                    x += ActiveFont.Measure(fragment.Text + " ").X * scale;
                }
                y += lineHeight;
            }
        }
    }


    FancyText text;
    static float Scale => 0.8f * TweakMenuManager.Layout.SubSettingScale;
    static float Offset;
    public float TextWidth;

    public Description(WebScrapper.TweakInfo info, string setting = null, float? textWidth = null)
    {
        Offset = setting != null ? TweakMenuManager.Layout.LeftOffset
            : TweakMenuManager.Layout.LeftOffset + TweakMenuManager.Layout.SubSettingOffset;

        if (setting == null)
        {
            if(info.tweakDescription != null)
                text = new FancyText(info.tweakDescription, () => 0.8f * TweakMenuManager.Layout.TweakScale);
        }
        else
        {
            if(info.settingDescs != null && info.settingDescs.TryGetValue(setting, out string settingDesc))
            {
                text = new FancyText(settingDesc, () => Scale);
            }
            else
            {
                string log = $"\nfailed loading {setting}";

                if (info.settingDescs != null && info.settingDescs.Count > 0)
                {
                    log += "\nexisting settings:";
                    foreach (var desc in info.settingDescs)
                        log += $"\n\"{desc.Key}\" - \"{desc.Value}\"";
                }
            }
        }

        Selectable = false;
        Disabled = true;
    }

    public override float Height() => text.String == "" ? 0f : text.Height + 4f;
    public override void Render(Vector2 position, bool selected)
    {
        position.X = Offset;
        text.Render(position);
    }
}