using Celeste.Mod.LeniencyHelper.Module;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using VivHelper.Entities;

namespace Celeste.Mod.LeniencyHelper.UI;

public class AbstractTweakItem : TextMenu.Item
{
    public string Label;
    protected Tweak tweak;
    protected int lastDir = 0;
    protected bool ViolateLeniency = false;
    private float sineCounter;
    protected float SineShift => (float)Math.Sin(sineCounter * 4f);
    protected Description description;

    public int MenuIndex;

    private float? cachedHeight;
    public static readonly Color InactiveColor = Color.DarkSlateGray;
    public Color StrokeColor => Color.Black * Container.Alpha * Container.Alpha * Container.Alpha;
    public Color MainColor(bool selected) => OverrideMainColor.HasValue ? OverrideMainColor.Value :
        (Disabled ? InactiveColor : selected ? Container.HighlightColor : Color.White)
        * Container.Alpha;

    private Color cachedMainColor;
    private Color cachedStrokeColor;
    private Vector2 cachedPosition;
    private float cachedRightWidth;
    public Color? OverrideMainColor = null;

    protected static readonly Color UnselectedColor = Color.White;
    private AbstractTweakItem(Tweak tweak)
    {
        this.tweak = tweak;
        Selectable = true;
    }
    public AbstractTweakItem(Tweak tweak, string settingName = null) : this(tweak)
    {
        Label = settingName == null ? DialogUtils.Lookup(tweak) : DialogUtils.Lookup(tweak, settingName);

        if (WebScrapper.TweaksInfo.ContainsKey(tweak))
            description = new Description(() => Container.Alpha, WebScrapper.TweaksInfo[tweak], settingName);
    }

    public override void Update()
    {
        base.Update();
        sineCounter += Monocle.Engine.RawDeltaTime;
    }

    public virtual void ChangeValue(int dir) { }
    public virtual bool TryChangeValue(int dir) => true;

    protected void OnLeniencyViolation() { }

    public override void LeftPressed()
    {
        lastDir = -1;
        if (!TryChangeValue(-1)) OnLeniencyViolation();
    }

    public override void RightPressed()
    {
        lastDir = 1;
        if (!TryChangeValue(1)) OnLeniencyViolation();
    }

    public override string SearchLabel() => Label;
    

    public override float Height()
    {
        if (cachedHeight != null) return cachedHeight.Value;
        cachedHeight = ActiveFont.LineHeight + (description?.Height() ?? 0f);
        return cachedHeight.Value;
    }


    #region Render
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


    public virtual void RenderDescription(Vector2 at)
    {
        description?.Render(at);
    }

    public virtual void Render(Vector2 position, bool selected, string value, bool left, bool right, Color? overrideColor = null)
    {
        position.X = TweakMenuManager.Layout.LeftOffset + TweakMenuManager.Layout.SubSettingOffset;

        if (selected) RenderDescription(position);
        position += Height() * Vector2.UnitY / 2f;

        cachedPosition = position;
        cachedRightWidth = RightWidth();
        cachedMainColor = MainColor(selected);
        cachedStrokeColor = StrokeColor;


        DrawLabel();

        DrawRightText(value, scale: 0.8f);

        float sineShift = selected ? SineShift : 0f;

        DrawRightText("<", -20f - cachedRightWidth / 2f - (left ? sineShift : 0f), inactiveColor: !left);
        DrawRightText(">", 20f + cachedRightWidth / 2f + (right ? sineShift : 0f), inactiveColor: !right);
    }
    #endregion
}