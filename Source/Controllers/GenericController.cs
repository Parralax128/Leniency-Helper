using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.LeniencyHelper.Controllers;
public abstract class GenericController : Entity
{
    public string stopFlag;
    public bool persistent;
    private TransitionListener transitionListener;

    private bool? prevFlagActive = null;
    public bool GetFlagActive => stopFlag != "" && SceneAs<Level>().Session.GetFlag(stopFlag);
    private bool removeOthers;

    public GenericController(EntityData data, Vector2 offset, bool removeOthers) : base(data.Position + offset)
    {
        this.removeOthers = removeOthers;

        stopFlag = data.Attr("StopFlag", "");
        persistent = data.Bool("Persistent", true);
        Add(transitionListener = new TransitionListener());
        transitionListener.OnOutBegin = OnLeave;
    }
    public override void Added(Scene scene)
    {
        base.Added(scene);

        if (!removeOthers) return;
        
        foreach(Entity e in Scene.Entities)
        {
            if(e is GenericController controller)
            {
                if (!controller.Equals(this) && controller.GetType() == this.GetType())
                {
                    Logger.Warn("LeniencyHelper", $"removed {this.GetType().Name} clone!");
                    controller.RemoveSelf();
                }
            }
        }
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (!persistent)
        {
            GetOldSettings();
        }

        Apply(false);
    }

    public override void Update()
    {
        base.Update();

        bool flagActiveNow = GetFlagActive;

        if (prevFlagActive.HasValue && flagActiveNow != prevFlagActive)
        {
            if (flagActiveNow) Undo(true);
            else Apply(true);
        }

        prevFlagActive = GetFlagActive;
    }

    public abstract void Apply(bool fromFlag);
    public abstract void Undo(bool fromFlag);
    public abstract void GetOldSettings();

    private void OnLeave()
    {
        if (!persistent)
        {
            Undo(false);
        }
    }
}