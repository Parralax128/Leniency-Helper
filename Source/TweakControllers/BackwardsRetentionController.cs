using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;

namespace Celeste.Mod.LeniencyHelper.TweakControllers;

[CustomEntity("LeniencyHelper/Controllers/BackwardsRetention")]
public class BackwardsRetentionController : GenericTweakController
{
    public BackwardsRetentionController(EntityData data, Vector2 offset) : base(data, offset, "BackwardsRetention")
    {

    }
}