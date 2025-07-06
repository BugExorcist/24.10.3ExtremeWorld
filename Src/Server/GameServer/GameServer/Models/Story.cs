using Common;
using Common.Data;
using GameServer.Managers;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    internal class Story
    {
        const float READY_TIME = 11f;
        const float ROUND_TIME = 60f;
        const float RESULT_TIME = 5f;

        public Map Map;
        public int StoryId;
        public int InstanceId;
        public NetConnection<NetSession> Player;

        Map SourceMap;

        int startPoint = 12;

        public int Round { get; internal set; }
        float timer = 0f;


        public Story(Map map, int storyId, int instanceId, NetConnection<NetSession> owner)
        {
            this.StoryId = storyId;
            this.Player = owner;
            this.Map = map;
        }

        internal void PlayerEnter()
        {
            this.SourceMap = PlayerLeaveMap(Player);
            this.PlayerEnterArena();
        }

        /// <summary>
        /// 发送角色离开地图的信息并且获取角色离开前的地图信息
        /// </summary>
        private Map PlayerLeaveMap(NetConnection<NetSession> player)
        {
            Map currentMap = MapManager.Instance[player.Session.Character.Info.mapId];
            currentMap.CharacterLeave(player.Session.Character);
            EntityManager.Instance.RemoveMapEntity(currentMap.ID, currentMap.InsanceID, player.Session.Character);
            return currentMap;
        }

        private void PlayerEnterArena()
        {
            TeleporterDefine startPoint = DataManager.Instance.Teleporters[this.startPoint];
            this.Player.Session.Character.Position = startPoint.Position;
            this.Player.Session.Character.Direction = startPoint.Direction;

            this.Map.AddCharacter(this.Player, this.Player.Session.Character);
            this.Map.CharacterEnter(this.Player, this.Player.Session.Character);

            EntityManager.Instance.AddMapEntity(this.Map.ID, this.Map.InsanceID, this.Player.Session.Character);
        }

        internal void Update()
        {

        }

        public void End()
        {

        }

    }
}
