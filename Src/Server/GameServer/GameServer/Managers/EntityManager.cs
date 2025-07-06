using Common;
using GameServer.Core;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EntityManager : Singleton<EntityManager>
    {
        private int idx = 0;
        public Dictionary<int, Entity> AllEntities = new Dictionary<int, Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        /// <summary>
        /// 获取地图索引唯一值
        /// </summary>
        public int GetMapIdx(int mapId, int instanceId)
        {   // 假设每个地图实例ID小于1000
            return mapId * 1000 + instanceId;
        }

        public void AddEntity(int mapId, int instanceId, Entity entity)
        {
            //加入管理器生成唯一ID 
            //因为角色和怪物都属于entity需要被加载到地图中，但是只有每个角色有DB ID，怪物没有，所以使用EntityID
            entity.EntityData.Id = ++this.idx;
            AllEntities.Add(entity.EntityData.Id, entity);
            AddMapEntity(mapId, instanceId, entity);
        }

        internal void AddMapEntity(int mapId, int instanceId, Entity entity)
        {
            List<Entity> entities = null;
            int index = GetMapIdx(mapId, instanceId);
            if (!MapEntities.TryGetValue(index, out entities))
            {
                entities = new List<Entity>();
                MapEntities[index] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, int instanceId, Entity entity)
        {
            this.AllEntities.Remove(entity.entityId);
            this.RemoveMapEntity(mapId, instanceId, entity);
        }

        internal void RemoveMapEntity(int mapId, int instanceId, Entity entity)
        {
            int index = GetMapIdx(mapId, instanceId);
            this.MapEntities[index].Remove(entity);
        }

        internal Entity GetEntity(int entityId)
        {
            Entity result = null;
            this.AllEntities.TryGetValue(entityId, out result);
            return result;
        }

        internal Creature GetCreature(int entityId)
        {
            return GetEntity(entityId) as Creature;
        }

        public List<T> GetMapEntitise<T>(int mapId, Predicate<Entity> match) where T : Creature
        {
            List<T> result = new List<T>();
            if (!this.MapEntities.TryGetValue(this.GetMapIdx(mapId, 0), out List<Entity> entities)) return null;
            foreach (Entity entity in entities)
            {
                if (entity is T && match.Invoke(entity))
                {
                    result.Add(entity as T);
                }
            }
            return result;
        }

        public List<T> GetMapEntitiseInRange<T>(int mapId, Vector3Int pos, int range) where T : Creature
        {
            return this.GetMapEntitise<T>(mapId, (entity) =>
            {
                T creature = entity as T;
                return creature.Distance(pos) < range;
            });
        }

        
    }
}
