using Common.Data;
using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using System;

namespace Managers
{
    public class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        internal void Init(List<NItemInfo> items)
        {
            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);

                Debug.LogFormat("ItemManager:Init[{0}]", item);
            }
            StatusService.Instance.RegisterStatuesNotify(StatusType.Item, OnItemNotify);
        }

        private bool OnItemNotify(NStatus status)
        {
            if (status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }
            else if(status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }
            return true;
        }
        private void AddItem(int id, int value)
        {
            Item item = null;
            if (this.Items.TryGetValue(id, out item))
            {
                item.Count += value;
            }
            else
            {
                item = new Item(id, value);
                this.Items.Add(id, item);
            }
            BagManager.Instance.AddItem(id, value);
        }

        private void RemoveItem(int id, int value)
        {
            if (!this.Items.ContainsKey(id))
            {
                return;
            }
            else
            {
                Item item = this.Items[id];
                if (item.Count < value)
                {
                    return;
                }
                item.Count -= value;
                BagManager.Instance.RemoveItem(id, value);
            }
        }

        public ItemDefine GetItem(int itenId)
        {
            return null;
        }

        public bool UseItem(int itemId)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }


    }
}