using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    internal class Arena
    {
        public Map Map;
        public ArenaInfo ArenaInfo;
        public NetConnection<NetSession> Red;
        public NetConnection<NetSession> Blue;

        //红蓝两队的原始地图
        Map SourseMapRed;
        Map SourseMapBlue;

        public Arena(Map map, ArenaInfo arena, NetConnection<NetSession> red, NetConnection<NetSession> blue)
        {
            this.ArenaInfo = arena;
            this.Red = red;
            this.Blue = blue;
            this.Map = map;
            arena.ArenaId = map.InsanceID;
        }

        internal void PlayerEnder()
        {
            this.SourseMapRed = PlayerLeaveMap(this.Red);
            this.SourseMapBlue = PlayerLeaveMap(this.Blue);

        }


        private Map PlayerLeaveMap(NetConnection<NetSession> player)
        {
            Map currentMap = MapManager.Instance[player.Session.Character.Info.mapId];
            currentMap.CharacterLeave(player.Session.Character);
            EntityManager.Instance.RemoveMapEntity(player.Session.Character.entityId);
            return currentMap;
        }
    }
}
