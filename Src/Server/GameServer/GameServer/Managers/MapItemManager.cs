using System;
using System.Collections.Generic;
using Common.Data;
using GameServer.Models;
using SkillBridge.Message;

namespace GameServer.Managers
{
    public class MapItemManager
    {
        private Dictionary<int, MapItem> items = new Dictionary<int, MapItem>();
        private int nextId = 1;
        private Map Owner;

        public void Init(Map map)
        {
            this.Owner = map;
        }

        public void AddItem(int itemId, int count, NVector3 position)
        {
            MapItem item = new MapItem()
            {
                MapItemId = nextId++,
                ItemId = itemId,
                Count = count,
                Position = position
            };
            this.items.Add(item.MapItemId, item);
            
            MapItemSpawnNotify notify = new MapItemSpawnNotify();
            notify.Items.Add(item.ToMessage());
            
            this.Owner.Broadcast(notify);
        }

        public void RemoveItem(int mapItemId)
        {
            if (items.ContainsKey(mapItemId))
            {
                items.Remove(mapItemId);
                
                MapItemRemoveNotify notify = new MapItemRemoveNotify();
                notify.mapItemId = mapItemId;
                this.Owner.Broadcast(notify);
            }
        }

        public List<NMapItem> GetItems()
        {
            List<NMapItem> result = new List<NMapItem>();
            foreach (var item in items.Values)
            {
                result.Add(item.ToMessage());
            }
            return result;
        }

        public MapItem GetItem(int mapItemId)
        {
            if (items.ContainsKey(mapItemId))
                return items[mapItemId];
            return null;
        }
    }
}
