using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;

namespace Entities
{
    public class Entity
    {
        public int entityId;


        public Vector3Int position;
        public Vector3Int direction;
        public int speed;
        public bool ready = true;
        public IEntityController Controller;

        private NEntity entityData;
        public NEntity EntityData
        {
            get
            {
                UpdateEntityData();
                return entityData;
            }
            set
            {
                entityData = value;
                entityId = entityData.Id;
                this.SetEntityData(value);
            }
        }

        public Entity(NEntity entity)
        {
            this.SetEntityData(entity);
        }

        public virtual void OnUpdate(float delta)
        {
            if (this.speed != 0)
            {
                Vector3 dir = this.direction;
                this.position += Vector3Int.RoundToInt(dir * speed * delta / 100f);
            }
        }

        public void SetEntityData(NEntity entity)
        {
            if (!ready) return;
            this.entityId = entity.Id;
            this.entityData = entity;
            this.position = this.position.FromNVector3(entity.Position);
            this.direction = this.direction.FromNVector3(entity.Direction);
            this.speed = entity.Speed;
        }
        internal void UpdateEntityData()
        {
            entityData.Position.FromVector3Int(this.position);
            entityData.Direction.FromVector3Int(this.direction);
            entityData.Speed = this.speed;
        }
    }
}
