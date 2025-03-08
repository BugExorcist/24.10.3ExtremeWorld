using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Diagnostics;

namespace GameServer.Services
{
    class MapServices : Singleton<MapServices>
    {
        public MapServices()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapCharacterEnterRequest>(this.OnMapCharacterEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
        }

        public void Init()
        {
            MapManager.Instance.Init();
        }

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapEntitySync: CharacterID:{0} : {1} Entity.Id: {2} Evt:{3} Entity:{4}", character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event,request.entitySync.Entity.ToString());

            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);
        }

        private void OnMapCharacterEnter(NetConnection<NetSession> sender, MapCharacterEnterRequest request)
        {
            throw new NotImplementedException();
        }

        internal void SendEntityUpdate(NetConnection<NetSession> connection, NEntitySync entity)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapEntitySync = new MapEntitySyncResponse();
            message.Response.mapEntitySync.entitySyncs.Add(entity);

            byte[] data = PackageHandler.PackMessage(message);
            connection.SendData(data, 0, data.Length);
        }
    }
}
