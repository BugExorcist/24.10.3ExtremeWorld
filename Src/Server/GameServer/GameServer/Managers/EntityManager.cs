using Common;
using GameServer.Core;
using GameServer.Entities;
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


        public void AddEntity(int mapId, Entity entity)
        {
            //加入管理器生成唯一ID 
            //因为角色和怪物都属于entity需要被加载到地图中，但是只有每个角色有DB ID，怪物没有，所以使用EntityID
            entity.EntityData.Id = ++this.idx;
            AllEntities.Add(entity.EntityData.Id, entity);

            List<Entity> entities = null;
            if (!MapEntities.TryGetValue(mapId, out entities))
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, Entity entity)
        {
            this.AllEntities.Remove(entity.entityId);
            this.MapEntities[mapId].Remove(entity);
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
            foreach (Entity entity in this.MapEntities[mapId])
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
