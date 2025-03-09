using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using Common.Data;
using Managers;

namespace Services
{
    internal class MapService : Singleton<MapService>, IDisposable
    {
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
            

        }

        public int CurrentMapId { get; set; }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

        }

        public void Init()
        {

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                if (User.Instance.CurrentCharacter == null || User.Instance.CurrentCharacter.Id == cha.Id)
                {
                    User.Instance.CurrentCharacter = cha;
                }
                CharacterManager.Instance.AddCharacter(cha);
            }
            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave: CharID:{0}", response.characterId);
            if (response.characterId != User.Instance.CurrentCharacter.Id)
                CharacterManager.Instance.RemoveCharacter(response.characterId);
            else
                CharacterManager.Instance.Clear();
        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
            }
            else
                Debug.LogErrorFormat("EnterMap：Map {0} not existed", mapId);
        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity)
        {
            Debug.LogFormat("MapEntityUpdateRequest ID:{0} Position:{1} Direction:{2} Speed:{3}", entity.Id, entity.Position.ToString(), entity.Direction.ToString(), entity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Entity = entity,
                Id = entity.Id,
                Event = entityEvent
            };
            NetClient.Instance.SendMessage(message);
        }
        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
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
    }
}
