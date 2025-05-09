using Common;
using Common.Data;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class Spawner
    {
        public SpawnRuleDefine Define { get; set; }

        private Map Map;
        // 怪物刷新时间
        private float spaenTime = 0;
        // 怪物消失时间
        private float unspaenTime = 0;

        private bool spawned = false;

        private SpawnPointDefine spawnPoint = null;

        public Spawner(SpawnRuleDefine define1, Map map)
        {
            this.Define = define1;
            this.Map = map;

            if (DataManager.Instance.SpawnPoints.ContainsKey(this.Map.ID))
            {
                if (DataManager.Instance.SpawnPoints[this.Map.ID].ContainsKey(this.Define.SpawnPoint))
                {
                    spawnPoint = DataManager.Instance.SpawnPoints[this.Map.ID][this.Define.SpawnPoint];
                }
                else
                {
                    Log.ErrorFormat("SpaenRule[{0}] SpawnPoint[{1}] not existed", this.Define.ID, this.Define.SpawnPoint);
                }
            }
        }

        public void Updata()
        {
            if (this.CanSpawn())
            {
                this.Spawn();
            }
        }

        private bool CanSpawn()
        {
            if (this.spawned)
                return false;
            if (this.unspaenTime + this.Define.SpawnPeriod > Time.time)
                return false;
            return true;
        }

        private void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map[{0}]Spawn[{1}:Mon{2},Lv:{3}] At Point{4}", this.Define.MapID, this.Define.ID, this.Define.SpawnMonID, this.Define.SpawnLevel, this.Define.SpawnPoint);
            this.Map.MonsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }
    }
}