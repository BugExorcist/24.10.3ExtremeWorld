using Common;
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
        public List<Entity>AllEntities = new List<Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();


        public void AddEntity(int mapId, Entity entity)
        {
            AllEntities.Add(entity);
            //加入管理器生成唯一ID 
            //因为角色和怪物都属于entity需要被加载到地图中，但是只有每个角色有DB ID，怪物没有，所以使用EntityID
            entity.EntityData.Id = ++this.idx;

            List<Entity> entities = null;
            if(!MapEntities.TryGetValue(mapId, out entities))
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, Entity entity)
        {
            this.AllEntities.Remove(entity);
            this.MapEntities[mapId].Remove(entity);
        }
    }
}
