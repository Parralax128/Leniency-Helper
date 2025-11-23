using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.ComponentModel;
using System.Reflection.Emit;
using VivHelper.Entities;

namespace Celeste.Mod.LeniencyHelper.UI;

public class TweakSetting<T> : TextMenu.Option<T>
{
    public T value;
    private Description description;
    public string settingName;

    public static readonly Color InactiveColor = Color.DarkSlateGray;
    public Color StrokeColor => Color.Black * Container.Alpha * Container.Alpha * Container.Alpha;
    public Color MainColor(bool highlighted) => Disabled ? InactiveColor : (highlighted ? Container.HighlightColor : UnselectedColor) * Container.Alpha;
    private Color cachedMainColor;
    private Color cachedStrokeColor;
    private Vector2 cachedPosition;
    private float cachedRightWidth;

    public TweakSetting(string tweak, string settingName, TextMenu addedTo, bool dontSetValue = false) : base(FromDialog(settingName, true))
    {
        if(!dontSetValue) value = SettingMaster.GetSetting<T>(settingName, tweak);
        this.settingName = settingName;
        string searchSetting = FromDialog(settingName.ToLower().Contains("inframes") ? "CountInFrames" : settingName, false);

        if (WebScrapper.TweaksInfo.ContainsKey(tweak))
        {
            description = new Description(addedTo, WebScrapper.TweaksInfo[tweak], searchSetting);
        }
        else
        {
            description = null;
        }
    }
    private static string FromDialog(string str, bool label)
    {
        if (str.ToLower().Contains("inframe")) str = "CountInFrames";
        return Dialog.Clean("MODOPTIONS_LENIENCYHELPER_SETTINGS_" + str.ToUpper(),
            label ? null : Dialog.Languages["english"]);
    }
    public override float Height()
    {
        if (description == null || Container.Current != this) return base.Height();
        return base.Height() + description.GetHeight();
    }

    public Vector2 RightColumn(Vector2 orig, Vector2 offset = default)
    {
        return new Vector2(Engine.ViewWidth - TweakMenuManager.Layout.RightOffset - cachedRightWidth
            + (lastDir < 0 ? (0f - ValueWiggler.Value) * 8f : 0f), orig.Y) + offset;
    }
    public void DrawTextCentered(string text, Vector2 position, float scale = 1f, bool inactiveColor = false)
    {
        ActiveFont.DrawOutline(text, position, new Vector2(0.5f), Vector2.One * scale,
            inactiveColor ? InactiveColor * Container.Alpha : cachedMainColor, 2f, cachedStrokeColor);
    }
    public void DrawRightText(string text, float offsetX = 0f, float scale = 1f, bool inactiveColor = false)
    {
        DrawTextCentered(text, RightColumn(cachedPosition, Vector2.UnitX * offsetX), scale, inactiveColor);
    }
    public void DrawLabel()
    {
        ActiveFont.DrawOutline(Label, cachedPosition, new Vector2(0f, 0.5f), Vector2.One, cachedMainColor, 2f, cachedStrokeColor);
    }

    public void BeforeRender(ref Vector2 position, bool selected)
    {
        position.X = TweakMenuManager.Layout.LeftOffset + Engine.ViewWidth * 0.03f;
        if (selected) description?.Render(position, Container.Width);
        position += Height() * Vector2.UnitY / 2f;
        
        cachedPosition = position;
        cachedRightWidth = RightWidth();
        cachedMainColor = MainColor(selected);
        cachedStrokeColor = StrokeColor;
    }
}