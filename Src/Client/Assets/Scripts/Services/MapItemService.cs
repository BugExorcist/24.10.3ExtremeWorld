using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class MapItemService : Singleton<MapItemService>, IDisposable
    {
        public delegate void MapItemSpawnHandler(List<NMapItem> items);
        public event MapItemSpawnHandler OnMapItemSpawn;

        public delegate void MapItemRemoveHandler(int mapItemId);
        public event MapItemRemoveHandler OnMapItemRemove;

        public void Init()
        {
            MessageDistributer.Instance.Subscribe<MapItemSpawnNotify>(this.OnMapItemSpawnNotify);
            MessageDistributer.Instance.Subscribe<MapItemRemoveNotify>(this.OnMapItemRemoveNotify);
            MessageDistributer.Instance.Subscribe<MapItemPickupResponse>(this.OnMapItemPickupResponse);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapItemSpawnNotify>(this.OnMapItemSpawnNotify);
            MessageDistributer.Instance.Unsubscribe<MapItemRemoveNotify>(this.OnMapItemRemoveNotify);
            MessageDistributer.Instance.Unsubscribe<MapItemPickupResponse>(this.OnMapItemPickupResponse);
        }

        /// <summary>
        /// 地图掉落物更新
        /// </summary>
        private void OnMapItemSpawnNotify(object sender, MapItemSpawnNotify message)
        {
            if (this.OnMapItemSpawn != null)
            {
                this.OnMapItemSpawn(message.Items);
            }
        }

        private void OnMapItemRemoveNotify(object sender, MapItemRemoveNotify message)
        {
            if (this.OnMapItemRemove != null)
            {
                this.OnMapItemRemove(message.mapItemId);
            }
        }
        
        public void SendPickupRequest(int mapItemId)
        {
            Debug.Log($"SendPickupRequest: {mapItemId}");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapItemPickup = new MapItemPickupRequest();
            message.Request.mapItemPickup.mapItemId = mapItemId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnMapItemPickupResponse(object sender, MapItemPickupResponse message)
        {
            if (message.Result == Result.Success)
            {
                Debug.Log("Pickup Success");
            }
            else
            {
                Debug.LogError($"Pickup Failed: {message.Errormsg}");
            }
        }
    }
}
