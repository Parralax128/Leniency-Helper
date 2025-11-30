using Monocle;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Celeste.Mod.LeniencyHelper.UI;

public static class TutorialPlayer
{
    private static VideoPlayer player = new() { IsLooped = true, IsMuted = true };
    public static Video CurrentVideo;

    private static Rectangle SourceRectangle;

    public static void LoadVideo(string tweakName)
    {
        string path = $"Graphics/Videos/{tweakName}.ogv";

        if(!Everest.Content.TryGet(path, out ModAsset asset))
        {
            Logger.Error(Module.LeniencyHelperModule.Instance.Metadata.Name, $"No file found with path: {path}");
            return;
        }

        CurrentVideo = Engine.Instance.Content.Load<Video>(asset.GetCachedPath());
        SourceRectangle = new Rectangle(0, 0, CurrentVideo.Width, CurrentVideo.Height);
    }
    public static void PlayTutorial()
    {
        //player.Play(CurrentVideo);
    }


    public static void Render(Vector2 position, bool highlighted)
    {
        //Draw.SpriteBatch.Draw(player.GetTexture(), Vector2.Zero, Color.White);
    }
}