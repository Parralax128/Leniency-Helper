using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.UI;

class FancyText
{
    struct TextFragment
    {
        public string Text;
        public Color Color;
    }

    // Formatting tag definitions
    static readonly Dictionary<string, Action<FormattingState>> TagActions = new()
    {
        { "ins", state => state.Color = Color.SlateGray }
    };

    // State tracker for the parser
    class FormattingState
    {
        public Color Color = Color.DarkSlateGray; // Default 'NormalColor'
        public float? Scale = null;
    }

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
        parsedFragments.Clear();
        var currentState = new FormattingState();
        var currentText = new System.Text.StringBuilder();
        bool insideTag = false;
        var tagBuffer = new System.Text.StringBuilder();

        for (int i = 0; i < String.Length; i++)
        {
            char c = String[i];

            if (c == '<' && !insideTag)
            {
                // Save the fragment before the tag
                if (currentText.Length > 0)
                {
                    parsedFragments.Add(new TextFragment
                    {
                        Text = currentText.ToString(),
                        Color = currentState.Color,
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
                {
                    currentState.Color = Color.DarkSlateGray;
                    currentState.Scale = null;
                }
                else if (TagActions.TryGetValue(tag, out var action))
                {
                    action.Invoke(currentState);
                }
                continue;
            }

            if (insideTag)
            {
                tagBuffer.Append(c);
            }
            else
            {
                currentText.Append(c);
            }
        }

        if (currentText.Length > 0)
        {
            parsedFragments.Add(new TextFragment
            {
                Text = currentText.ToString(),
                Color = currentState.Color,
            });
        }
    }

    void WrapLines()
    {
        wrappedLines.Clear();
        List<TextFragment> currentLine = new();
        float currentX = 0f;

        foreach (var fragment in parsedFragments)
        {
            string[] words = fragment.Text.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words)
            {
                // Handle explicit newline (if the word is actually empty from a split)
                if (word == "\n" || word.Contains("\n"))
                {
                    // Force a new line
                    if (currentLine.Count > 0) wrappedLines.Add(new List<TextFragment>(currentLine));
                    currentLine.Clear();
                    currentX = 0f;
                    continue;
                }

                float wordWidth = ActiveFont.Measure(word + ' ').X * EffectiveScale;

                if (currentX + wordWidth > LineWidth && currentLine.Count > 0)
                {
                    wrappedLines.Add(new List<TextFragment>(currentLine));
                    currentLine.Clear();
                    currentX = 0f;
                }

                currentLine.Add(new TextFragment
                {
                    Text = word,
                    Color = fragment.Color
                });
                currentX += wordWidth;
            }
        }

        if (currentLine.Count > 0)
        {
            wrappedLines.Add(currentLine);
        }
    }


    public void Render(Vector2 position, float alpha = 1f)
    {
        position.Y -= Height / 2f;
        float lineHeight = ActiveFont.LineHeight * EffectiveScale;
        float y = position.Y;

        foreach (List<TextFragment> line in wrappedLines)
        {
            float x = position.X;
            foreach (TextFragment fragment in line)
            {
                float fragmentScale = EffectiveScale;
                ActiveFont.DrawOutline(
                    fragment.Text + " ", // Add the space back for rendering
                    new Vector2(x, y),
                    new Vector2(0f, 0f),
                    Vector2.One * fragmentScale,
                    fragment.Color,
                    2f,
                    Color.Black
                );
                x += ActiveFont.Measure(fragment.Text + " ").X * fragmentScale;
            }
            y += lineHeight;
        }
    }
}