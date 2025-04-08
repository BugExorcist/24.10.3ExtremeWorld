using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using Common.Data;
using Managers;
using UnityEditor.PackageManager.UI;

namespace Services
{
    internal class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponce>(this.OnItemBuy);
        }

        public int CurrentMapId { get; set; }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponce>(this.OnItemBuy);
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

        private void OnItemBuy(object sender, ItemBuyResponce responce)
        {
            MessageBox.Show("购买结果：" + responce.Result + "\n" + responce.Errormsg, "购买完成");
        }
    }
}
