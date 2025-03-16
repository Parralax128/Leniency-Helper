using System;
using Monocle;
using Celeste;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using FMOD;
using IL.MonoMod;
using MonoMod.RuntimeDetour;
using System.Reflection;
using static Monocle.VirtualButton;
using Microsoft.Win32.SafeHandles;
using System.Linq;
using System.ComponentModel;
using Microsoft.VisualBasic.FileIO;

namespace Celeste.Mod.LeniencyHelper.Triggers
{
    [CustomEntity("LeniencyHelper/SetupTheoOnRespawnTrigger")]
    public class SetupEntityOnRespawnTrigger : Trigger
    {
        private Vector2 checkPos;
        private Vector2 newEntityPosition;
        private Vector2 newEntitySpeed;
        private Entity[] affectedEntities = Array.Empty<Entity>();
        private string blackList = "";
        private string whiteList = "";
        private string varName;
        private bool varValue;
        public SetupEntityOnRespawnTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            checkPos = new Vector2
                (data.Nodes[0].X + offset.X, data.Nodes[0].Y + offset.Y);
            
            newEntityPosition = new Vector2
                (data.Nodes[1].X + offset.X, data.Nodes[1].Y + offset.Y);
            
            newEntitySpeed = new Vector2(data.Int("SpeedX", (int)Position.X),
                data.Int("SpeedY", (int)Position.Y));

            blackList = data.Attr("Blacklist", "");
            whiteList = data.Attr("Whitelist", "");

            varName = data.Attr("VariableName", "");
            varValue = data.Bool("Value", false);
        }
        public override void Update()
        {
            base.Update();

            Player player = scene.Tracker.GetEntity<Player>();
            if (player is not null && CollideCheck(player) && !player.JustRespawned
                && affectedEntities.Length > 0)
            {
                foreach (Entity entity in affectedEntities)
                {
                    entity.Center = newEntityPosition;

                    PropertyInfo speedProperty = entity.GetType().GetProperty("Speed");
                    if (speedProperty is null)
                        speedProperty = entity.GetType().GetProperty("Speed", BindingFlags.NonPublic);
                    FieldInfo speedField = entity.GetType().GetField("Speed");
                    if (speedField is null)
                        speedField = entity.GetType().GetField("Speed", BindingFlags.NonPublic);


                    if (speedField is not null &&
                        speedField.FieldType.ToString().Contains("Vector2"))
                    {
                        speedField.SetValue(entity, newEntitySpeed);
                    }

                    else if (speedProperty is not null &&
                        speedProperty.PropertyType.ToString().Contains("Vector2"))
                    {
                        speedProperty.SetValue(entity, newEntitySpeed);
                    }


                    if(varName != "")
                    {
                        PropertyInfo varProperty = entity.GetType().GetProperty(varName);
                        if (varProperty is null)
                            varProperty = entity.GetType().GetProperty(varName, BindingFlags.NonPublic);
                        FieldInfo varField = entity.GetType().GetField(varName);
                        if(varField is null)
                            varField = entity.GetType().GetField(varName, BindingFlags.NonPublic);

                        if(varField is not null &&
                            varField.FieldType.ToString().ToLower() == "system.boolean")
                        {
                            varField.SetValue(entity, varValue);
                        }
                        if(varProperty is not null &&
                            varProperty.PropertyType.ToString().ToLower() == "system.boolean")
                        {
                            varProperty.SetValue(entity, varValue);
                        }

                    }

                    affectedEntities = Array.Empty<Entity>();
                }
            }
            else if (player is not null && player.JustRespawned && affectedEntities.Length > 0)
            {
                foreach (Entity entity in affectedEntities)
                {
                    entity.Center = newEntityPosition;
                }
            }
        }
        private Scene scene;
        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            this.scene = scene;
            affectedEntities = Array.Empty<Entity>();

            foreach (Entity entity in scene.Entities)
            {
                if (entity is not Trigger && entity is not SolidTiles && entity is not null)
                {
                    if (entity.CollidePoint(checkPos))
                    {
                        string type = entity.GetType().Name;
                        if (!blackList.Contains(type))
                        {
                            if (whiteList == "" || whiteList.Contains(type))
                            {
                                affectedEntities = affectedEntities.Append(entity).ToArray();
                                entity.Center = newEntityPosition;
                            }
                        }
                    }
                }

            }
        }
    }
}