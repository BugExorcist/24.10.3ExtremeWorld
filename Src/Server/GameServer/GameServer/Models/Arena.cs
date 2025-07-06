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
    internal class Arena
    {
        const float READY_TIME = 11f;
        const float ROUND_TIME = 60f;
        const float RESULT_TIME = 5f;

        public Map Map;
        public ArenaInfo ArenaInfo;
        public NetConnection<NetSession> Red;
        public NetConnection<NetSession> Blue;

        //红蓝两队的原始地图
        Map SourseMapRed;
        Map SourseMapBlue;

        // 红蓝两队的传送点ID
        const int RedPotin = 9;
        const int BluePotin = 10;

        // 红蓝两队是否准备好
        private bool redReady;
        private bool blueReady;
        public bool Ready
        {
            get { return this.redReady && this.blueReady; }
        }

        private ArenaStatus ArenaStatus;
        private ArenaRoundStatus RoundStatus;
        public int Round;

        float timer = 0f;

        TeleporterDefine redPoint = DataManager.Instance.Teleporters[RedPotin];
        TeleporterDefine bluePoint = DataManager.Instance.Teleporters[BluePotin];

        private int redScore;
        private int blueScore;

        public Arena(Map map, ArenaInfo arena, NetConnection<NetSession> red, NetConnection<NetSession> blue)
        {
            this.ArenaInfo = arena;
            this.ArenaInfo.Winner = -1;
            this.Red = red;
            this.Blue = blue;
            this.Map = map;
            arena.ArenaId = map.InsanceID;
            this.ArenaStatus = ArenaStatus.Wait;
            this.RoundStatus = ArenaRoundStatus.None;
            this.blueScore = 0;
            this.redScore = 0;
        }

        internal void PlayerEnter()
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
            InitPosition();
            //this.Map.AddCharacter(this.Red, this.Red.Session.Character);
            //this.Map.AddCharacter(this.Blue, this.Blue.Session.Character);

            this.Map.CharacterEnter(this.Red, this.Red.Session.Character);
            this.Map.CharacterEnter(this.Blue, this.Blue.Session.Character);

            EntityManager.Instance.AddMapEntity(this.Map.ID, this.Map.InsanceID, this.Red.Session.Character);
            EntityManager.Instance.AddMapEntity(this.Map.ID, this.Map.InsanceID, this.Blue.Session.Character);
        }

        private void InitPosition()
        {
            this.Red.Session.Character.Position = redPoint.Position;
            this.Red.Session.Character.Direction = redPoint.Direction;
            this.Blue.Session.Character.Position = bluePoint.Position;
            this.Blue.Session.Character.Direction = bluePoint.Direction;
        }

        internal void Update()
        {
            if (this.ArenaStatus == ArenaStatus.Game)
            {
                UpdateRound();
            }
        }

        private void UpdateRound()
        {
            if (this.RoundStatus == ArenaRoundStatus.Ready)
            {
                this.timer -= Time.deltaTime;
                if (this.timer <= 0)
                {
                    this.RoundStatus = ArenaRoundStatus.Fight;
                    this.timer = ROUND_TIME;
                    Log.InfoFormat("Arena:[{0}] Round Start", this.ArenaInfo.ArenaId);
                    ArenaService.Instance.SendArenaRoundStart(this);
                }
            }
            else if (this.RoundStatus == ArenaRoundStatus.Fight)
            {
                CaculateResult();
            }
            else if (this.RoundStatus == ArenaRoundStatus.Result)
            {
                this.timer -= Time.deltaTime;
                if (this.timer <= 0)
                {
                    if (this.Round >= 3)
                    {
                        ArenaResult();
                    }
                    else
                    {
                        NextRound();
                    }
                }
            }
        }

        private void CaculateResult()
        {
            this.timer -= Time.deltaTime;
            if (this.timer <= 0)
            {   //超时
                this.RoundStatus = ArenaRoundStatus.Result;
                this.timer = RESULT_TIME;
                this.ArenaInfo.Rounds.Add(new ArenaRoundInfo()
                {
                    Round = this.Round,
                    Winner = -1
                });
                Log.InfoFormat("Arena:[{0}] Round End", this.ArenaInfo.ArenaId);
                if (this.Round == 3)
                {
                    ArenaResult();
                }
                ArenaService.Instance.SendArenaRoundEnd(this);

            }
            else if (this.Blue.Session.Character.Attributes.HP <= 0)
            {   //红方获胜
                this.RoundStatus = ArenaRoundStatus.Result;
                this.timer = RESULT_TIME;
                this.ArenaInfo.Rounds.Add(new ArenaRoundInfo()
                {
                    Round = this.Round,
                    Winner = 0
                });
                this.redScore++;
                Log.InfoFormat("Arena:[{0}] Round End", this.ArenaInfo.ArenaId);
                if (this.Round == 3)
                {
                    ArenaResult();
                }
                ArenaService.Instance.SendArenaRoundEnd(this);
            }
            else if (this.Red.Session.Character.Attributes.HP <= 0)
            {   //蓝方获胜
                this.RoundStatus = ArenaRoundStatus.Result;
                this.timer = RESULT_TIME;
                this.ArenaInfo.Rounds.Add(new ArenaRoundInfo()
                {
                    Round = this.Round,
                    Winner = 1
                });
                this.blueScore++;
                Log.InfoFormat("Arena:[{0}] Round End", this.ArenaInfo.ArenaId);
                if (this.Round == 3)
                {
                    ArenaResult();
                }
                ArenaService.Instance.SendArenaRoundEnd(this);
            }
        }

        private void ArenaResult()
        {
            Log.InfoFormat("Arena:[{0}] Result", this.ArenaInfo.ArenaId);
            this.ArenaStatus = ArenaStatus.Resule;
            // TODO: 结算
            if (this.blueScore < this.redScore)
            {
                this.ArenaInfo.Winner = 0;
            }
            else if (this.blueScore > this.redScore)
            {
                this.ArenaInfo.Winner = 1;
            }
            this.RoundStatus = ArenaRoundStatus.Result;
            
        }

        internal void EntityReady(int entityId)
        {
            if (this.Red.Session.Character.entityId == entityId)
            {
                this.redReady = true;
            }
            else if (this.Blue.Session.Character.entityId == entityId)
            {
                this.blueReady = true;
            }

            if (this.Ready)
            {
                this.ArenaStatus = ArenaStatus.Game;
                this.Round = 0;
                NextRound();
            }
        }

        private void NextRound()
        {
            this.Round++;
            this.timer = READY_TIME;
            this.RoundStatus = ArenaRoundStatus.Ready;

            Log.InfoFormat("Arena:{[0]} Round{[1]} Ready", this.ArenaInfo.ArenaId, this.Round);
            ArenaService.Instance.SendArenaReday(this);

            RecoverAttribute();
        }

        private void RecoverAttribute()
        {
            this.Red.Session.Character.Attributes.RecoverHPAndMP();
            this.Blue.Session.Character.Attributes.RecoverHPAndMP();
        }
    }
}
