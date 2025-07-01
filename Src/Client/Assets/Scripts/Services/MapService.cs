using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using Common.Data;
using Managers;
using Entities;

namespace Services
{
    internal class MapService : Singleton<MapService>, IDisposable
    {
        public int CurrentMapId { get; set; }

        private bool loadingDown = true;
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);

            SceneManager.Instance.onSceneLoadDone += OnLoadDown;
        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
            SceneManager.Instance.onSceneLoadDone -= OnLoadDown;
        }

        public void Init()
        {

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                if (User.Instance.CurrentCharacterInfo == null || cha.Type == CharacterType.Player && User.Instance.CurrentCharacterInfo.Id == cha.Id)
                {   //如果是当前自机角色
                    User.Instance.CurrentCharacterInfo = cha;
                    if (User.Instance.CurrentCharacter == null)
                    {   //角色第一次进入游戏
                        User.Instance.CurrentCharacter = new Character(cha);
                    }
                    else
                    {
                        User.Instance.CurrentCharacter.UpdateInfo(cha);
                    }
                    User.Instance.CurrentCharacter.ready = false;
                    User.Instance.CharacterInited();
                    CharacterManager.Instance.AddCharacter(User.Instance.CurrentCharacter);

                    if (CurrentMapId != response.mapId)
                    {
                        this.EnterMap(response.mapId);
                        this.CurrentMapId = response.mapId;
                    }
                    continue;
                }
                CharacterManager.Instance.AddCharacter(new Character(cha));
            }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave: CharID:{0}", response.entityId);
            if (response.entityId != User.Instance.CurrentCharacterInfo.EntityId)
                CharacterManager.Instance.RemoveCharacter(response.entityId);
            else
            {
                if (User.Instance.CurrentCharacterObject != null)
                {
                    User.Instance.CurrentCharacterObject.OnLeaveLevel();
                }
                CharacterManager.Instance.Clear();

            }
        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                loadingDown = false;
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map; 
                SceneManager.Instance.LoadScene(map.Resource);
                SoundManager.Instance.PlayMusic(map.Music);
            }
            else
                Debug.LogErrorFormat("EnterMap：Map {0} not existed", mapId);
        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity, int param)
        {
            if (!loadingDown) return;
            Debug.LogFormat("MapEntityUpdateRequest EntityID:{0} Position:{1} Direction:{2} Speed:{3}", entity.Id, entity.Position.String(), entity.Direction.String(), entity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Entity = entity,
                Id = entity.Id,
                Event = entityEvent,
                Param = param
            };
            NetClient.Instance.SendMessage(message);
        }
        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            if (!loadingDown) return;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponce: Entity:{0}", response.entitySyncs.Count);
            sb.AppendLine();
            foreach (var entity in response.entitySyncs)
            {
                Managers.EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("   [{1}]evt:{1}  entity:{2}", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }

        internal void SendMapTeleporter(int teleproterID)
        {
            Debug.LogFormat("MapTeleporterRequest: teleproterID:{0}", teleproterID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleproterID;
            NetClient.Instance.SendMessage(message);
        }

        private void OnLoadDown()
        {
            if (User.Instance.CurrentCharacter != null)
                User.Instance.CurrentCharacter.ready = true;
            if (User.Instance.CurrentCharacterObject != null)
            {
                User.Instance.CurrentCharacterObject.OnEnterLevel();
            }
            this.loadingDown = true;
        }
    }
}
