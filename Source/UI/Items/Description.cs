using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper.UI.Items;

class Description : TextMenu.Item
{
    static readonly Dictionary<string, ValueTuple<string, string>> MarkDownTags = new()
    {
        { "ins", ("#708090", "#") },
        { "em", ("#4f606b", "#") },
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

    string inputText;
    FancyText.Text Text;
    Coroutine coroutine;

    const float UnrollDelay = 0.25f;
    const float UnrollDuration = 0.5f;
    static readonly float Scale = 0.8f * TweakMenuManager.Layout.SubSettingScale;
    static readonly Vector2 ScaleFactor = new Vector2(Scale);

    float HorizontalOffset;
    const float VerticalOffset = 8f;

    const float CollapseDelay = 0.15f;
    const float CollapseDuration = 0.15f;

    UnrollCollapseHelper heightHelper;
    float maxHeight;

    int currentIndex;

    static DescriptionPos Mode => TweakMenuManager.Layout.DescriptionPos;
    public static bool AboveTweak => Mode == DescriptionPos.AboveTweak;

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

        HorizontalOffset = offsetX;

        float maxLine = Mode == DescriptionPos.AboveTweak ? (1920f - HorizontalOffset - layout.RightOffset) : layout.VideoSize.X;
        Text = FancyText.Parse(inputText = ReplaceMarkDownTags(text), (int)(maxLine / Scale), 1024, 0f, Color.DarkSlateGray, Dialog.Languages["english"]);

        float delayPerChar = UnrollDuration / Text.Count;
        foreach (FancyText.Char letter in Text.Nodes.Where(node => node is FancyText.Char))
            letter.Delay = delayPerChar;


        coroutine = new Coroutine();
        coroutine.Active = false;

        heightHelper = new UnrollCollapseHelper(CollapseDuration, UnrollDuration,
            (v) => Visible = v, coroutine, UnrollRoutine, CollapseRoutine);
        maxHeight = (Text.Lines + 1f) * Scale * ActiveFont.LineHeight + VerticalOffset;


        Selectable = false;
        Disabled = true;
    }


    public void SetVisible(bool value)
    {
        if (value) Unroll();
        else Collapse();
    }

    System.Collections.IEnumerator UnrollRoutine()
    {
        if(heightHelper.State != UnrollCollapseHelper.States.Collapsing)
        {
            yield return UnrollDelay;

            if (heightHelper.State == UnrollCollapseHelper.States.Collapsing)
                yield break;
        }

        heightHelper.State = UnrollCollapseHelper.States.Unrolling;

        float delayCounter = 0f;

        currentIndex = Math.Clamp(currentIndex, 0, Text.Count - 1);
        while (currentIndex < Text.Count)
        {
            FancyText.Node current = Text[currentIndex];
            if (current is FancyText.Char letter)
            {
                bool longDelay = letter.Delay >= Engine.RawDeltaTime;

                if (!longDelay)
                {
                    letter.Fade = 1f;

                    delayCounter += letter.Delay;
                    if(delayCounter >= Engine.RawDeltaTime)
                    {
                        yield return null;
                        delayCounter = 0f;
                    }
                }
                else
                {
                    while (letter.Fade < 1f)
                    {
                        letter.Fade = Math.Min(1f, letter.Fade + Engine.RawDeltaTime / letter.Delay);
                        yield return null;
                    }
                }

                currentIndex++;
            }
            else if (current is FancyText.Wait delay)
            {
                yield return delay.Duration;
                currentIndex++;
            }
            else
            {
                yield return null;
                currentIndex++;
            }
        }
    }
    public void Unroll() => heightHelper.Unroll();
    System.Collections.IEnumerator CollapseRoutine()
    {
        if (heightHelper.State != UnrollCollapseHelper.States.Unrolling) yield return CollapseDelay;
        heightHelper.State = UnrollCollapseHelper.States.Collapsing;

        float delayCounter = 0f;

        currentIndex = Math.Clamp(currentIndex, 0, Text.Count - 1);
        while (currentIndex >= 0)
        {
            FancyText.Node current = Text[currentIndex];
            if (current is FancyText.Char letter)
            {
                bool longDelay = letter.Delay >= Engine.RawDeltaTime;

                if (!longDelay)
                {
                    letter.Fade = 0f;

                    delayCounter += letter.Delay;
                    if (delayCounter >= Engine.RawDeltaTime)
                    {
                        yield return null;
                        delayCounter = 0f;
                    }
                }
                else
                {
                    while (letter.Fade > 0f)
                    {
                        letter.Fade = Math.Max(0f, letter.Fade - Engine.RawDeltaTime/letter.Delay);
                        yield return null;
                    }
                }

                currentIndex--;
            }
            else if (current is FancyText.Wait delay)
            {
                yield return delay.Duration;
                currentIndex--;
            }
            else
            {
                yield return null;
                currentIndex--;
            }
        }
    }
    public void Collapse() => heightHelper.Collapse();

    public override void Update() => heightHelper.Update();

    public override float Height() => Mode == DescriptionPos.UnderPlayer ? 0f : heightHelper.GetHeight(Visible, maxHeight);
    
    public override void Render(Vector2 position, bool selected)
    {
        position = Mode switch
        {
            DescriptionPos.AboveTweak => new Vector2(HorizontalOffset, position.Y - Height() / 2f + VerticalOffset),
            DescriptionPos.UnderPlayer => TweakMenuManager.Layout.DescUnderVideo - Vector2.UnitY * (maxHeight - heightHelper.GetHeight(Visible, maxHeight)),
            _ => throw new InvalidOperationException("what the fuck?!")
        };

        Text.Draw(position, Vector2.Zero, ScaleFactor, Container.Alpha);
    }
    public void RebuildText()
    {
        var layout = TweakMenuManager.Layout;
        float maxLine = Mode == DescriptionPos.AboveTweak ? (1920f - HorizontalOffset - layout.RightOffset) / Scale : layout.VideoSize.X;
        Text = FancyText.Parse(ReplaceMarkDownTags(inputText), (int)maxLine, 1024, 0f, Color.DarkSlateGray, Dialog.Languages["english"]);
    }

    [Command("desc", "")]
    public static void Switch()
    {
        TweakMenuManager.Layout.DescriptionPos = 1 - TweakMenuManager.Layout.DescriptionPos;
    }
}