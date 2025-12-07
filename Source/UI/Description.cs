using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.LeniencyHelper.UI;

public class Description : TextMenu.Item
{
    private FancyText text = new FancyText("");
    private static float Scale => 0.8f * TweakMenuManager.Layout.SubSettingScale;
    private static float Offset;
    public float TextWidth;

    public Description(WebScrapper.TweakInfo info, string setting = null, float? textWidth = null)
    {
        Offset = setting == null ? 0f : TweakMenuManager.Layout.LeftOffset + TweakMenuManager.Layout.SubSettingOffset;

        if (setting == null)
        {
            text = new FancyText(info.tweakDescription, () => 0.8f * TweakMenuManager.Layout.TweakScale);
        }
        else
        {
            try { text = new FancyText(info.settingDescs[setting], () => Scale); }
            catch
            {
                Debug.Warn($"failed loading {setting}");

                if(info.settingDescs != null && info.settingDescs.Count > 0) {
                    Debug.Log("existing settings:");
                    foreach (var desc in info.settingDescs)
                        Debug.Log($"\"{desc.Key}\" - \"{desc.Value}\"");
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
        text.Render(position, Container.Alpha);
    }
}