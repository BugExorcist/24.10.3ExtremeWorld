using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class SpawnManager
    {
        private List<Spawner> Rules = new List<Spawner>();

        private Map Map;
        public void Init(Map map)
        {
            this.Map = map;
            if (DateManager.Instance.SpawnRules.ContainsKey(map.Define.ID))
            {
                foreach (var define in DateManager.Instance.SpawnRules[map.Define.ID].Values)
                    this.Rules.Add(new Spawner(define, this.Map));
            }
        }

        internal void Updata()
        {
            if (Rules.Count > 0)
            {
                for (int i = 0; i < this.Rules.Count;i++)
                {
                    this.Rules[i].Updata();
                }
            }
        }
    }
}
