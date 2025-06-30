using Common;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {
        //第一个key是地图ID， 第二个key是实例ID（副本）   没有副本的地图只有一个实例
        Dictionary<int, Dictionary<int, Map>> Maps = new Dictionary<int, Dictionary<int, Map>>();

        public void Init()
        {
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", mapdefine.ID, mapdefine.Name);

                int instanceCount = 1;
                if (mapdefine.Type == Common.Data.MapType.Arena)
                {
                    instanceCount = ArenaManager.MaxInstance;
                }
                this.Maps[mapdefine.ID] = new Dictionary<int, Map>();
                for (int i = 0; i < instanceCount; i++)
                {
                    this.Maps[mapdefine.ID][i] = new Map(mapdefine, i);
                }
            }
        }

        /// <summary>
        /// 返回地图第一个实例
        /// </summary>
        public Map this[int key]
        {
            get
            {
                return this.Maps[key][0];
            }
        }
        /// <summary>
        /// 返回指定的地图实例
        /// </summary>
        internal Map GetInstance(int arenaMapId, int instanceId)
        {
            return this.Maps[arenaMapId][instanceId];
        }

        public void Update()
        {
            foreach (var maps in this.Maps.Values)
            {
                foreach (var instance in maps.Values)
                {
                    instance.Update();
                }
            }
        }
    }
}