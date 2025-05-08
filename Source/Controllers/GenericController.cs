using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Controllers;
public abstract class GenericController : Entity
{
    //
    public string stopFlag;
    public bool persistent;
    private TransitionListener transitionListener;

    private bool? prevFlagActive = null;
    private bool GetFlagActive => stopFlag != "" && SceneAs<Level>().Session.GetFlag(stopFlag);

    public GenericController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        stopFlag = data.Attr("StopFlag", "");
        persistent = data.Bool("Persistent", true);
        Add(transitionListener = new TransitionListener());
        transitionListener.OnOutBegin = OnLeave;
    }
    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (!persistent)
        {
            GetOldSettings();
        }

        ApplyTweak();
        ApplySettings();
    }

    public override void Update()
    {
        base.Update();

        bool flagActiveNow = GetFlagActive;

        if (prevFlagActive.HasValue && flagActiveNow != prevFlagActive)
        {
            if (flagActiveNow) UndoTweak();
            else ApplyTweak();
        }

        prevFlagActive = GetFlagActive;
    }

    public abstract void GetOldSettings();
    public abstract void ApplySettings();

    private void OnLeave()
    {
        if (!persistent)
        {
            UndoTweak();
            UndoSettings();
        }
    }
    public abstract void UndoSettings();
    public abstract void UndoTweak();
    public abstract void ApplyTweak();
}