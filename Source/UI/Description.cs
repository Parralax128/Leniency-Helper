using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.LeniencyHelper.UI;

public class Description
{
    private static readonly Color Color = Color.DarkSlateGray;

    private Func<float> AlphaGetter;
    private string text = "";
    private const float Scale = 0.7f;


    public Description(Func<float> getAlpha, WebScrapper.TweakInfo info, string settingName = null)
    {
        if (settingName == null)
        {
            text = info.tweakDescription;
        }
        else
        {
            try
            {
                text = info.settingDescs[settingName];
            }
            catch
            {
                Logging.Warn($"failed loading {settingName}");

                if(info.settingDescs != null && info.settingDescs.Count > 0)
                {
                    Logging.Warn("existing settings:");
                    foreach (var desc in info.settingDescs)
                    {
                        Logging.Log($"\"{desc.Key}\" - \"{desc.Value}\"");
                    }
                }
            }
        }

        AlphaGetter = getAlpha;
        var layout = TweakMenuManager.Layout;
        if(text != null) text = SplitText((Monocle.Engine.Width - layout.LeftOffset - layout.RightOffset) * 0.7f);
    }
    private string SplitText(float maxLineLen)
    {
        string[] words = text.Split(' ');
        string result = "";

        float currentLineLen = 0f;

        foreach(string word in words)
        {
            currentLineLen += ActiveFont.Measure(word + ' ').X;
            if (currentLineLen >= maxLineLen)
            {
                result += '\n';
                currentLineLen = 0f;
            }
            result += word + ' ';
        }
        return result;
    }


    public float GetHeight()
    {
        return text == "" ? 0f : ActiveFont.Measure(text).Y * Scale + 4f;
    }
    public void Render(Vector2 position, float textWidth)
    {
        float alpha = AlphaGetter.Invoke();

        ActiveFont.DrawOutline(text, 
            position,
            new Vector2(0f, 0.5f), 
            new Vector2(Scale),
            Color * alpha, 
            2f, Color.Black * alpha * alpha * alpha);
    }
}