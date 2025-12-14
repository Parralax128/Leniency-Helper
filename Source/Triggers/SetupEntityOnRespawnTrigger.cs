using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[CustomEntity("LeniencyHelper/SetupEntityOnRespawnTrigger")]
class SetupEntityOnRespawnTrigger : Trigger
{
    Vector2 checkPos;
    Vector2 newEntityPosition;
    Vector2 newEntitySpeed;
    List<Entity> affectedEntities = new List<Entity>();
    List<string> blackList = new List<string>();
    List<string> whiteList = new List<string>();
    public SetupEntityOnRespawnTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        checkPos = new Vector2
            (data.Nodes[0].X + offset.X, data.Nodes[0].Y + offset.Y);

        newEntityPosition = new Vector2
            (data.Nodes[1].X + offset.X, data.Nodes[1].Y + offset.Y);

        newEntitySpeed = new Vector2(data.Int("SpeedX", (int)Position.X),
            data.Int("SpeedY", (int)Position.Y));

        // Split?
        blackList = (data.Attr("Blacklist", "")).Replace(" ", "").Split(',').ToList();
        whiteList = GenericTrigger.StringToList(data.Attr("Whitelist", ""));
    }
    public override void Update()
    {
        base.Update();

        if(Scene.Tracker.GetEntity<Player>()?.JustRespawned == false)
        {
            foreach (Entity entity in affectedEntities)
            {
                entity.Center = newEntityPosition;

                PropertyInfo speedProperty = entity.GetType().GetProperty("Speed");
                if (speedProperty == null) speedProperty = entity.GetType().GetProperty("Speed", BindingFlags.NonPublic);

                FieldInfo speedField = entity.GetType().GetField("Speed");
                if (speedField == null) speedField = entity.GetType().GetField("Speed", BindingFlags.NonPublic);


                if (speedField != null && speedField.FieldType == typeof(Vector2))
                {
                    speedField.SetValue(entity, newEntitySpeed);
                }
                else if (speedProperty != null && speedProperty.PropertyType == typeof(Vector2))
                {
                    speedProperty.GetSetMethod().Invoke(entity, new object[] { newEntitySpeed });
                }
            }

            affectedEntities.Clear();
            RemoveSelf();
        }
        else
        {
            foreach (Entity entity in affectedEntities) 
                entity.Center = newEntityPosition;            
        }
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);

        affectedEntities.Clear();

        Player player = scene.Tracker.GetEntity<Player>();
        
        if (player != null && !CollideCheck(player))
        {
            RemoveSelf();
            return;
        }

        foreach (Entity entity in scene.Entities)
        {
            if (entity is not Trigger && entity is not SolidTiles && entity != null)
            {
                bool saveCollidable = entity.Collidable;
                entity.Collidable = true;
                if (entity.CollidePoint(checkPos))
                {
                    string type = entity.GetType().Name;
                    if (!blackList.Contains(type))
                    {
                        if (whiteList.Count == 0 || whiteList.Contains(type))
                        {
                            affectedEntities.Add(entity);
                            entity.Center = newEntityPosition;
                        }
                    }
                }
                entity.Collidable = saveCollidable;
            }
        }
    }
}