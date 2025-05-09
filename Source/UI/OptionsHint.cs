using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.UI;

public class OptionsHint : TextMenu.Item
{
    private static string confirmTutorial = Dialog.Clean("LENIENCYHELPER_OPTIONS_CONFIRMHINT");
    private static string grabTutorial = Dialog.Clean("LENIENCYHELPER_OPTIONS_GRABHINT");
    private static MTexture confirmButton => Input.GuiButton(Input.MenuConfirm, Input.PrefixMode.Latest);
    private static MTexture grabButton => Input.GuiButton(Input.Grab, Input.PrefixMode.Latest);

    public OptionsHint()
    {
        Selectable = false;
    }

    public override float Height() => confirmButton.Height + grabButton.Height;

    public override void Render(Vector2 position, bool highlighted = false)
    {
        position.Y -= Height() / 2f;

        confirmButton.DrawJustified(position, Vector2.Zero);
        position.Y += confirmButton.Height / 2f;

        ActiveFont.DrawOutline(confirmTutorial, position + Vector2.UnitX * (confirmButton.Width + 10f),
            new Vector2(0f, 0.5f), Vector2.One * 0.7f, Color.DarkSlateGray, 2, Color.Black);

        position.Y += confirmButton.Height / 2f;

        grabButton.DrawJustified(position, Vector2.Zero);
        position.Y += grabButton.Height / 2f;

        ActiveFont.DrawOutline(grabTutorial, position + Vector2.UnitX * (confirmButton.Width + 10f),
            new Vector2(0f, 0.5f), Vector2.One * 0.7f, Color.DarkSlateGray, 2, Color.Black);
    }
}
