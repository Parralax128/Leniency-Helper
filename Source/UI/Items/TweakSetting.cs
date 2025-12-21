using Microsoft.Xna.Framework;
using System;
using Celeste.Mod.LeniencyHelper.TweakSettings;
using System.Collections.Generic;
using Celeste.Mod.LeniencyHelper.UI.Items.SettingHandlers;

namespace Celeste.Mod.LeniencyHelper.UI.Items;

class TweakSetting<T> : AbstractTweakItem
{
    protected Setting<T> Setting;
    AbstractHandler<T> handler;

    public bool PlayerSource = false;

    static readonly Dictionary<Type, object> Handlers = new()
    {
        { typeof(bool), new BoolHandler() },
        { typeof(int), new IntHandler() },
        { typeof(float), new FloatHandler() },
        { typeof(Time), new TimeHandler() },
    };
    static AbstractHandler<T> GetHandler()
    {
        Type type = typeof(T);
        if (Handlers.TryGetValue(type, out var handler))
            return (AbstractHandler<T>)handler;

        if (type.IsEnum)
        {
            Type handlerType = typeof(EnumHandler<>).MakeGenericType(type);
            var enumHadler = Activator.CreateInstance(handlerType);
            Handlers.Add(type, enumHadler);
            return (AbstractHandler<T>)enumHadler;
        }

        throw new ArgumentException($"Could not find a corresponding SettingTypeHandler for the type \"{type.Name}\"");
    }


    public TweakSetting(Tweak tweak, Setting<T> setting) : base(tweak, setting.Name)
    {
        Setting = setting;

        handler = GetHandler();
        cachedWidth = handler.CalculateMaxWidth(Setting);
        OnAltPressed += () => { handler.OnJournalPressed?.Invoke(Setting); };

        TextScale = Layout.SubSettingScale;
    }

    public override bool TryChangeValue(int dir)
    {
        return handler.CheckValidDir(Setting.Player, Setting.ValueBounds, dir);
    }
    public override void ChangeValue(int dir)
    {
        base.ChangeValue(dir);
        Setting.Player = handler.Advance(Setting.Player, dir);
    }
    public override void ConfirmPressed()
    {
        Setting.Reset(SettingSource.Player);
    }
    public override float RightWidth() => cachedWidth;
    public override void Render(Vector2 position, bool selected)
    {
        handler.CheckBounds(Setting, out bool left, out bool right);
        position.X = Layout.LeftOffset + Layout.SubSettingOffset;
        base.Render(position, selected, handler.GetText(Setting.Get(TweakData.Tweaks[tweak].CurrentSettingSource)), left, right);
    }
}