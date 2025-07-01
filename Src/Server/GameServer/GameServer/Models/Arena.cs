using Common.Data;
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

        // 红蓝两队的传送点ID
        int RedPotin = 9;
        int BluePotin = 10;

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

            this.PlayerEnterArena();
        }

        /// <summary>
        /// 发送角色离开地图的信息并且获取角色离开前的地图信息
        /// </summary>
        private Map PlayerLeaveMap(NetConnection<NetSession> player)
        {
            Map currentMap = MapManager.Instance[player.Session.Character.Info.mapId];
            currentMap.CharacterLeave(player.Session.Character);
            EntityManager.Instance.RemoveMapEntity(currentMap.ID, currentMap.InsanceID ,player.Session.Character);
            return currentMap;
        }

        private void PlayerEnterArena()
        {
            TeleporterDefine redPoint = DataManager.Instance.Teleporters[this.RedPotin];
            this.Red.Session.Character.Position = redPoint.Position;
            this.Red.Session.Character.Direction = redPoint.Direction;
            TeleporterDefine bluePoint = DataManager.Instance.Teleporters[this.BluePotin];
            this.Blue.Session.Character.Position = bluePoint.Position;
            this.Blue.Session.Character.Direction = bluePoint.Direction;

            //this.Map.AddCharacter(this.Red, this.Red.Session.Character);
            //this.Map.AddCharacter(this.Blue, this.Blue.Session.Character);

            this.Map.CharacterEnter(this.Red, this.Red.Session.Character);
            this.Map.CharacterEnter(this.Blue, this.Blue.Session.Character);

            EntityManager.Instance.AddMapEntity(this.Map.ID, this.Map.InsanceID, this.Red.Session.Character);
            EntityManager.Instance.AddMapEntity(this.Map.ID, this.Map.InsanceID, this.Blue.Session.Character);
        }
    }
}
