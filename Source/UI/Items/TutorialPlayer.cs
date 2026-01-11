using Monocle;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Celeste.Mod.LeniencyHelper.UI.Items;

public class TutorialPlayer : TextMenu.Item
{
    static VideoPlayer player = new() { IsLooped = true, IsMuted = true };
    public static Video CurrentVideo = null;

    static Rectangle SourceRectangle;

    public TutorialPlayer()
    {
        Selectable = false;
        Disabled = true;
    
        AboveAll = true;
    }

    public override bool AlwaysRender => true;

    public static void LoadVideo(Tweak tweak)
    {
        string path = $"Graphics/Videos/{tweak}.ogv";

        if(!Everest.Content.TryGet(path, out ModAsset asset))
        {
            Logger.Error(Module.LeniencyHelperModule.Name, $"No file found with path: {path}");
            CurrentVideo = null;
            return;
        }

        CurrentVideo = Engine.Instance.Content.Load<Video>(asset.GetCachedPath());
        SourceRectangle = new Rectangle(0, 0, CurrentVideo.Width, CurrentVideo.Height);

        player.Play(CurrentVideo);
    }

    public override float Height() => 0f;
    public override void Render(Vector2 position, bool selected)
    {
        if (CurrentVideo == null) return;
        Draw.SpriteBatch.Draw(player.GetTexture(), TweakMenuManager.Layout.VideoDestination, SourceRectangle, Color.White);
    }
}