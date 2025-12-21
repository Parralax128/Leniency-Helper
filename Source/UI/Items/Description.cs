using Monocle;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper.UI.Items;

class Description : TextMenu.Item
{
    static readonly Dictionary<string, ValueTuple<string, string>> MarkDownTags = new()
    {
        { "ins", ("#708090", "#") },
        { "em", ("%", "/%") },
        { "br", ("n", "n") }
    };

    static string ReplaceMarkDownTags(string text)
    {
        string result = new string(text);
        foreach(KeyValuePair<string, ValueTuple<string, string>> pair in MarkDownTags)
        {
            result = result.Replace($"<{pair.Key}>", $"{{{pair.Value.Item1}}}");
            result = result.Replace($"</{pair.Key}>", $"{{{pair.Value.Item2}}}");
        }
        return result;
    }

    FancyText.Text Text;
    Coroutine coroutine;
    int currentLine = 0;
    
    float heightWigglerCounter = 0f;
    const float HeightWiggleDuration = 0.5f;
    float? staticHeight = null;

    static readonly float Scale = 0.8f * TweakMenuManager.Layout.SubSettingScale;
    float Offset;
    bool selected = false;

    public static string? GetText(WebScrapper.TweakInfo info, string setting = null)
    {
        if (setting == null) return info.tweakDescription;
        
        if (info.settingDescs != null
            && info.settingDescs.TryGetValue(setting, out string settingDesc)
            && !string.IsNullOrEmpty(settingDesc))
        {
            return settingDesc;
        }


        string log = $"\nfailed loading {setting}";

        if (info.settingDescs != null && info.settingDescs.Count > 0)
        {
            log += "\nexisting settings:";
            foreach (var desc in info.settingDescs)
                log += $"\n\"{desc.Key}\" - \"{desc.Value}\"";
        }

        return null;
    }

    public Description(string text, float offsetX)
    {
        MenuLayout layout = TweakMenuManager.Layout;

        Offset = offsetX;

        Text = FancyText.Parse(
            ReplaceMarkDownTags(text),
            (int)((1920f-Offset-layout.RightOffset) / Scale),
            1024, 0f,
            Color.DarkSlateGray,
            Dialog.Languages["english"]);

        coroutine = new();
        coroutine.Active = false;

        Selectable = false;
        Disabled = true;
    }

    System.Collections.IEnumerator TextAppearRoutine()
    {
        foreach(FancyText.Char letter in Text.Nodes.Where(node => node is FancyText.Char))
        {
            letter.Fade = 0f;
            letter.Delay = 0.001f;
        }

        int index = 0;
        while(index < Text.Count)
        {
            FancyText.Node current = Text[index];
            if (current is FancyText.Char letter)
            {
                if(letter.Line > currentLine)
                {
                    currentLine = letter.Line;
                    heightWigglerCounter = 0f;
                }

                while (letter.Fade < 1f) 
                {
                    letter.Fade = Math.Min(1f, letter.Fade + Engine.RawDeltaTime / letter.Delay);
                    yield return null;
                }
                index++;
            }
            else if (current is FancyText.Wait delay)
            {
                yield return delay.Duration;
                index++;
            }
            else
            {
                yield return null;
                index++;
            }
        }
    }
    public override void Update()
    {
        if (Visible && !selected)
        {
            coroutine.Replace(TextAppearRoutine());
            staticHeight = null;
            currentLine = 0;
            heightWigglerCounter = 0f;
        }
        else if (Visible && selected)
        {   
            coroutine.Update();

            if (heightWigglerCounter < HeightWiggleDuration)
            {
                heightWigglerCounter += Engine.RawDeltaTime;

                if(heightWigglerCounter >= HeightWiggleDuration && currentLine == Text.Lines)
                    staticHeight = Height();
            }
        }

        selected = Visible;
    }

    const float e = 2.71828f;
    public override float Height()
    {
        if (staticHeight.HasValue) return staticHeight.Value;

        float x = heightWigglerCounter / HeightWiggleDuration;
        float wiggle = 1f - (float)Math.Pow(e, -(x*x * e*e));

        return (currentLine + wiggle) * ActiveFont.LineHeight * Scale + 8f;
    }
    public override void Render(Vector2 position, bool selected)
    {
        Text.Draw(new Vector2(Offset, position.Y - Height() / 2f + 4f),
            Vector2.Zero, new Vector2(Scale), 1f);
    }
}