using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Controllers;
public abstract class GenericController : Entity
{
    public string stopFlag;
    public bool persistent;
    TransitionListener transitionListener;

    bool? prevFlagActive = null;
    public bool FlagActive => stopFlag != "" && SceneAs<Level>().Session.GetFlag(stopFlag);

    public GenericController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        stopFlag = data.Attr("StopFlag", "");
        persistent = data.Bool("Persistent", true);
        Add(transitionListener = new TransitionListener());
        transitionListener.OnOutBegin = OnLeave;

        void OnLeave() 
        {
            if (!persistent)
                Undo(false);
        }
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (!persistent)
        {
            SaveData();
        }

        Apply(false);
    }

    public override void Update()
    {
        base.Update();

        bool flagActiveNow = FlagActive;

        if (prevFlagActive.HasValue && flagActiveNow != prevFlagActive)
        {
            if (flagActiveNow) Undo(true);
            else Apply(true);
        }

        prevFlagActive = FlagActive;
    }

    protected abstract void Apply(bool fromFlag);
    protected abstract void Undo(bool fromFlag);
    protected abstract void SaveData();
}