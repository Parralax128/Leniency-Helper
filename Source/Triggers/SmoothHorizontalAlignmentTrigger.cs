using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using Monocle;
using System.Runtime.CompilerServices;
using Celeste.Mod.Entities;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[CustomEntity("LeniencyHelper/SmoothHorizontalAlignmentTrigger")]
class SmoothHorizontalAlignmentTrigger : Trigger
{
    public enum Directions
    {
        Downwards = 1,
        Upwards = -1
    }
    public enum Easings
    {
        Linear,
        Sine,
        Quad,
        Cube,
        Quint,
        Exponent,
        Back,
        BigBack,
        Elastic
    }

    Directions fallDir;
    Ease.Easer easer;
    Vector2 enterPos;
    Vector2 target;
    float GetPercent(Vector2 playerPos)
    {
        if (fallDir == Directions.Downwards)
        {
            return Math.Clamp(1f - ((target.Y - playerPos.Y) / (target.Y - enterPos.Y)), 0f, 1f);
        }
        else return Math.Clamp(1f - ((playerPos.Y - target.Y) / (enterPos.Y - target.Y)), 0f, 1f);
    }
    
    
    string flag;
    bool FlagActive => flag == "" || SceneAs<Level>().Session.GetFlag(flag);
    bool oneUse;

    bool active;

    public SmoothHorizontalAlignmentTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        fallDir = data.Enum<Directions>("Direction", Directions.Downwards);
        easer = EnumToEaser(data.Enum<Easings>("Easing", Easings.Sine));
        flag = data.String("Flag", "");
        oneUse = data.Bool("OneUse", false);

        target = data.Nodes[0];
    }

    static Ease.Easer EnumToEaser(Easings enumValue) => enumValue switch {
        Easings.Linear => Ease.Linear,
        Easings.Sine => Ease.SineInOut,
        Easings.Quad => Ease.QuadInOut,
        Easings.Cube => Ease.CubeInOut,
        Easings.Quint => Ease.QuintInOut,
        Easings.Exponent => Ease.ExpoInOut,
        Easings.Back => Ease.BackInOut,
        Easings.BigBack => Ease.BigBackInOut,
        Easings.Elastic => Ease.ElasticInOut,
        _ => Ease.Linear
    };
    

    void Start(Vector2 from)
    {
        enterPos = from;
        active = true;
    }
    void Stop()
    {
        active = false;
        if (oneUse) RemoveSelf();
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Start(fallDir == Directions.Downwards ? player.TopCenter : player.BottomCenter);
    }
    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        Stop();
    }

    public override void Update()
    {
        Collidable = FlagActive;

        Player player = CollideFirst<Player>();
        if(active)
        {
            if (player == null || Math.Sign(player.Speed.Y) != (int)fallDir || !Collidable)
            {
                Stop();
            }
            else
            {
                player.MoveToX(float.Lerp(enterPos.X, target.X, easer(GetPercent(player.Center))), player.onCollideH);
            }
        }
        else
        {
            if(player != null && Math.Sign(player.Speed.Y) == (int)fallDir && Collidable)
            {
                Start(fallDir == Directions.Downwards ? player.TopCenter : player.BottomCenter);
            }
        }
    }
}