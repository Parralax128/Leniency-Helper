using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace Celeste.Mod.LeniencyHelper.UI;

public class Description
{
    private static readonly Color Color = Color.DarkSlateGray;

    private TextMenu Container;
    public string text = "";
    private const float Scale = 0.7f;
    
    public Description(TextMenu container, WebScrapper.TweakInfo info, string settingName = null)
    {
        Container = container;

        if (settingName == null)
        {
            text = info.description;
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
                    Logging.Warn("exisiting settings:");
                    foreach (var desc in info.settingDescs)
                    {
                        Logging.Log($"\"{desc.Key}\" - \"{desc.Value}\"");
                    }
                }
            }
        }

        if(text != null) text = SplitText(container.Left);
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
        float alpha = (float)Math.Pow(Container.Alpha, 3f);
        alpha = 1f;

        ActiveFont.DrawOutline(text, 
            position,
            new Vector2(0f, 0.5f), 
            Vector2.One * Scale,
            Color * alpha, 
            2f, Color.Black * alpha);
    }
}