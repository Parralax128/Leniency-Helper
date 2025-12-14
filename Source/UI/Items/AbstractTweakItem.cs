using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.LeniencyHelper.UI.Items;

class AbstractTweakItem : TextMenu.Item
{
    public TweakSlider Parent;
    public string Label;
    protected Tweak tweak;
    protected int lastDir = 0;
    protected bool ViolateLeniency = false;
    
    float sineCounter;
    protected float SineShift => (float)Math.Sin(sineCounter * 4f);
    public Description description { get; set; }

    public bool RenderDescription
    {
        set 
        { 
            if (description != null) description.Visible = value;
            if (Parent?.description != null) Parent.description.Visible = value;
        }
    }

    public static readonly Color InactiveColor = Color.DarkSlateGray;
    public Color StrokeColor => Color.Black * Container.Alpha * Container.Alpha * Container.Alpha;
    public Color MainColor(bool selected) => (Disabled ? InactiveColor : selected
        ? Container.HighlightColor : UnselectedColor) * Container.Alpha;
    protected static readonly Color UnselectedColor = Color.White;

    protected MenuLayout Layout => TweakMenuManager.Layout;

    Color cachedMainColor;
    Color cachedStrokeColor;
    Vector2 cachedPosition;    
    protected float cachedWidth;

    protected float TextScale = 1f;

    AbstractTweakItem(Tweak tweak)
    {
        this.tweak = tweak;
        Selectable = true;
    }
    public AbstractTweakItem(Tweak tweak, string settingName = null) : this(tweak)
    {
        Label = settingName == null ? DialogUtils.Lookup(tweak) : DialogUtils.Lookup(tweak, settingName);

        if (WebScrapper.TweaksInfo.ContainsKey(tweak))
        {
            description = new Description(WebScrapper.TweaksInfo[tweak], settingName != null ? DialogUtils.Lookup(tweak, settingName) : null);
            description.Visible = false;
        }
    }

    public override void Update()
    {
        base.Update();
        sineCounter += Engine.RawDeltaTime;

        if (description == null) return;
    }

    public virtual void ChangeValue(int dir) 
    { ValueWiggler.Start(); }

    public virtual bool TryChangeValue(int dir) => true;

    protected void OnLeniencyViolation() { }

    public override void LeftPressed()
    {
        lastDir = -1;
        if (TryChangeValue(-1)) 
            ChangeValue(-1);
    }

    public override void RightPressed()
    {
        lastDir = 1;
        if (TryChangeValue(1))
            ChangeValue(1);
    }

    public override string SearchLabel() => Label;
    

    public override float Height()
    {
        return ActiveFont.LineHeight * TextScale;
    }


    #region Render
    public Vector2 RightColumn(Vector2 orig, Vector2 offset = default)
    {
        return new Vector2(1920f - Layout.RightOffset
            + lastDir * ValueWiggler.Value * 8f, orig.Y) + offset;
    }

    public void DrawTextCentered(string text, Vector2 position, bool inactiveColor = false)
    {
        ActiveFont.DrawOutline(text, position, new Vector2(0.5f), Vector2.One * TextScale,
            inactiveColor ? InactiveColor * Container.Alpha : cachedMainColor, 2f, cachedStrokeColor);
    }
    public void DrawRightText(string text, float offsetX = 0f, bool inactiveColor = false)
    {
        DrawTextCentered(text, RightColumn(cachedPosition, Vector2.UnitX * offsetX), inactiveColor);
    }
    public void DrawLabel()
    {
        ActiveFont.DrawOutline(Label, cachedPosition, new Vector2(0f, 0.5f),
            Vector2.One * TextScale, cachedMainColor, 2f, cachedStrokeColor);
    }

    public virtual void Render(
        Vector2 position, bool selected,
        string value, bool left, bool right,
        Color? overrideColor = null)
    {
        cachedPosition = position;
        cachedMainColor = overrideColor ?? MainColor(selected);
        cachedStrokeColor = StrokeColor;

        DrawLabel();

        DrawRightText(value);

        float sineShift = selected ? SineShift : 0f;

        DrawRightText("<", -20f - cachedWidth / 2f - (left ? sineShift : 0f), inactiveColor: !left);
        DrawRightText(">", 20f + cachedWidth / 2f + (right ? sineShift : 0f), inactiveColor: !right);
    }
    #endregion
}