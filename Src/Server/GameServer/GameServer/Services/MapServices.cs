using Common;
using GameServer.Managers;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapServices : Singleton<MapServices>
    {
        public MapServices()
        {

        }
        public void Init()
        {
            MapManager.Instance.Init();
        }
    }
}
