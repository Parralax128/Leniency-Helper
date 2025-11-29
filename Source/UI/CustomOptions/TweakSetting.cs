using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.LeniencyHelper.UI.CustomOptions;

public class TweakSetting<T> : TextMenu.Option<T>
{
    public T value;
    private Description description;
    public string settingName;

    public static readonly Color InactiveColor = Color.DarkSlateGray;
    public Color StrokeColor => Color.Black * Container.Alpha * Container.Alpha * Container.Alpha;
    public Color MainColor(bool selected) => OverrideMainColor.HasValue ? OverrideMainColor.Value :
        (Disabled ? InactiveColor : selected ? Container.HighlightColor : UnselectedColor)
        * Container.Alpha;

    private Color cachedMainColor;
    private Color cachedStrokeColor;
    private Vector2 cachedPosition;
    private float cachedRightWidth;
    public Color? OverrideMainColor = null;

    public TweakSetting(Tweak tweak, string settingName, bool dontSetValue = false, bool isTweakSlider = false)
        : base(isTweakSlider ? TweakFromDialog(tweak) : SettingFromDialog(settingName, true))
    {
        if (!dontSetValue)
        {
            value = SettingMaster.GetSetting<T>(settingName, tweak);
        }

        this.settingName = settingName;

        description = WebScrapper.TweaksInfo.ContainsKey(tweak)
            ? new Description(() => Container.Alpha, WebScrapper.TweaksInfo[tweak],
                isTweakSlider ? null : SettingFromDialog(settingName, false))  : null;
    }

    private static string TweakFromDialog(Tweak str)
    {
        return Dialog.Clean("LENIENCYTWEAKS_" + str.ToString().ToUpper());
    }
    private static string SettingFromDialog(string str, bool notDataParse)
    {
        if (str.ToLower().Contains("inframe")) str = "CountInFrames";

        return Dialog.Clean("MODOPTIONS_LENIENCYHELPER_SETTINGS_" + str.ToUpper(),
            notDataParse ? null : Dialog.Languages["english"]);
    }

    private float cachedHeight;
    public override float Height()
    {
        if(Container.Items.Count > 0)
            cachedHeight = description == null || Container.Current != this ?
                base.Height() : base.Height() + description.GetHeight();
        
        return cachedHeight;
    }

    public Vector2 RightColumn(Vector2 orig, Vector2 offset = default)
    {
        return new Vector2(Engine.ViewWidth - TweakMenuManager.Layout.RightOffset
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

    public void BaseRender(string text, Vector2 position, bool selected, bool leftArrow, bool rightArrow)
    {
        position.X = TweakMenuManager.Layout.LeftOffset + 
            (settingName != null ? TweakMenuManager.Layout.SubSettingOffset : 0f);

        if (selected) description?.Render(position, Container.Width);
        position += Height() * Vector2.UnitY / 2f;
        
        cachedPosition = position;
        cachedRightWidth = RightWidth();
        cachedMainColor = MainColor(selected);
        cachedStrokeColor = StrokeColor;


        DrawLabel();

        DrawRightText(text, scale: 0.8f);

        float sineShift = selected ? (float)Math.Sin(sine * 4f) * 4f : 0f;

        DrawRightText("<", -20f - cachedRightWidth / 2f - (leftArrow ? sineShift : 0f), inactiveColor: !leftArrow);
        DrawRightText(">", 20f + cachedRightWidth / 2f + (rightArrow ? sineShift : 0f), inactiveColor: !rightArrow);
    }
}