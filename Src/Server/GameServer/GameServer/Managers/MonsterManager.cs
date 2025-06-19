using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    public class MonsterManager
    {
        private Map map;

        public Dictionary<int, Monster>Monsters = new Dictionary<int, Monster>();
        internal void Init(Map map)
        {
            this.map = map;
        }

        internal Monster Create(int spawnMonID, int spawnLevel, NVector3 position, NVector3 direction)
        {
            Monster monster = new Monster(spawnMonID, spawnLevel, position, direction);
            EntityManager.Instance.AddEntity(this.map.ID, monster);
            monster.Info.Id = monster.entityId;//怪物没有DBID
            monster.Info.EntityId = monster.entityId;
            monster.Info.mapId = this.map.ID;
            Monsters[monster.Id] = monster;
            
            this.map.MonsterEnter(monster);
            return monster;
        }
    }
}
