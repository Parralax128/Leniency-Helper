using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.LeniencyHelper.UI;

public class Description : TextMenu.Item
{
    private static readonly Color Color = Color.DarkSlateGray;

    private Func<float> AlphaGetter;
    private FancyText text = new FancyText("");
    private static float Scale => 0.8f * TweakMenuManager.Layout.SubSettingScale;
    public float TextWidth;

    public Description(Func<float> getAlpha, WebScrapper.TweakInfo info, string settingName = null, float? textWidth = null)
    {
        if (settingName == null)
        {
            text = new FancyText(info.tweakDescription, () => 0.8f * TweakMenuManager.Layout.TweakScale);
        }
        else
        {
            try { text = new FancyText(info.settingDescs[settingName], () => Scale); }
            catch {
                Debug.Warn($"failed loading {settingName}");

                if(info.settingDescs != null && info.settingDescs.Count > 0)
                {
                    Debug.Log("existing settings:");
                    foreach (var desc in info.settingDescs)
                    {
                        Debug.Log($"\"{desc.Key}\" - \"{desc.Value}\"");
                    }
                }
            }
        }

        AlphaGetter = getAlpha;
        var layout = TweakMenuManager.Layout;

        Selectable = false;
        Disabled = true;
    }

    public override float Height()
    {
        //if (!Visible) return 0f;
        return text.String == "" ? 0f : text.Height + 4f;
    }
    public override void Render(Vector2 position, bool selected)
    {
        //if (!Visible) return;
        float alpha = AlphaGetter.Invoke();

        position.X = TweakMenuManager.Layout.LeftOffset;
        text.Render(position);
    }
}