using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using Managers;

namespace Services
{
    internal class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);
            MessageDistributer.Instance.Subscribe<ItemUseResponse>(this.OnItemUse);
            MessageDistributer.Instance.Subscribe<ItemSplitResponse>(this.OnItemSplit);
            MessageDistributer.Instance.Subscribe<ItemDropResponse>(this.OnItemDrop);
        }

        public int CurrentMapId { get; set; }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);
            MessageDistributer.Instance.Unsubscribe<ItemUseResponse>(this.OnItemUse);
            MessageDistributer.Instance.Unsubscribe<ItemSplitResponse>(this.OnItemSplit);
            MessageDistributer.Instance.Unsubscribe<ItemDropResponse>(this.OnItemDrop);
        }

        public void SendBuyItem(int shopId, int shopItemId)
        {
            Debug.Log("SendBuyItem");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopItemId = shopItemId;
            message.Request.itemBuy.shopId = shopId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnItemBuy(object sender, ItemBuyResponse responce)
        {
            MessageBox.Show("购买结果：" + responce.Result + "\n" + responce.Errormsg, "购买完成");
        }

        Item pendingEquip = null;
        bool isEquip = false;
        public bool SendEquipItem(Item equip, bool isEquip)
        {
            if (pendingEquip != null)
                return false;
            Debug.Log("SendEquipItem");

            pendingEquip = equip;//用于记录当前在在操作的是哪个装备
            this.isEquip = isEquip;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();
            message.Request.itemEquip.Slot = (int)equip.EquipInfo.Slot;
            message.Request.itemEquip.itemId = equip.Id;
            message.Request.itemEquip.isEquip = isEquip;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnItemEquip(object sender, ItemEquipResponse message)
        {
            if (message.Result == Result.Success)
            {
                if(pendingEquip != null)
                {
                    if (this.isEquip)
                    {
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    }
                    else
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);
                    pendingEquip = null;
                }
            }
        }

        /// <summary>
        /// 发送使用物品请求
        /// </summary>
        public void SendUseItem(int slotIndex, int count = 1)
        {
            Debug.LogFormat("SendUseItem: SlotIndex:{0} Count:{1}", slotIndex, count);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemUse = new ItemUseRequest();
            message.Request.itemUse.slotIndex = slotIndex;
            message.Request.itemUse.Count = count;
            NetClient.Instance.SendMessage(message);
        }

        private void OnItemUse(object sender, ItemUseResponse message)
        {
            if (message.Result == Result.Success)
            {
                Debug.LogFormat("ItemUse Success: ItemId:{0}", message.itemId);
            }
            else
            {
                MessageBox.Show("使用失败：" + message.Errormsg, "提示");
            }
        }

        /// <summary>
        /// 发送拆分物品请求
        /// </summary>
        public void SendSplitItem(int fromSlot, int toSlot, int count)
        {
            Debug.LogFormat("SendSplitItem: FromSlot:{0} ToSlot:{1} Count:{2}", fromSlot, toSlot, count);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemSplit = new ItemSplitRequest();
            message.Request.itemSplit.fromSlot = fromSlot;
            message.Request.itemSplit.toSlot = toSlot;
            message.Request.itemSplit.Count = count;
            NetClient.Instance.SendMessage(message);
        }

        private void OnItemSplit(object sender, ItemSplitResponse message)
        {
            if (message.Result != Result.Success)
            {
                MessageBox.Show("拆分失败：" + message.Errormsg, "提示");
            }
        }

        /// <summary>
        /// 发送丢弃物品请求
        /// </summary>
        public void SendDropItem(int slotIndex, int count)
        {
            Debug.LogFormat("SendDropItem: SlotIndex:{0} Count:{1}", slotIndex, count);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemDrop = new ItemDropRequest();
            message.Request.itemDrop.slotIndex = slotIndex;
            message.Request.itemDrop.Count = count;
            NetClient.Instance.SendMessage(message);
        }

        private void OnItemDrop(object sender, ItemDropResponse message)
        {
            if (message.Result != Result.Success)
            {
                MessageBox.Show("丢弃失败：" + message.Errormsg, "提示");
            }
        }
    }
}
