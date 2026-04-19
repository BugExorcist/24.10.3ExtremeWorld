using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    class BagManager : Singleton<BagManager>
    {
        public int Unlocked;

        public BagItem[] Items;

        NBagInfo Info;

        public event Action OnUpdateItems;

        unsafe public void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[this.Unlocked];
            if (info.Items != null && info.Items.Length >= this.Unlocked)
            {
                Analyze(info.Items);
            }
            else
            {
                Info.Items = new byte[sizeof(BagItem) * this.Unlocked];
                Reset();
            }

            SaveToServer();
        }

        public void Reset()//整理背包
        {
            int i = 0;//背包格子索引
            foreach(var kv in ItemManager.Instance.Items)
            {
                if (kv.Value.Count <= kv.Value.Define.StackLimit)
                {
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)kv.Value.Count;
                }
                else
                {
                    int count = kv.Value.Count;
                    while(count > kv.Value.Define.StackLimit)
                    {
                        this.Items[i].ItemId = (ushort)kv.Key;
                        this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;
                        i++;
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)count;
                }
                i++;
            }

            OnUpdateItems?.Invoke();
            SaveToServer();
        }

        unsafe void Analyze(byte[] data)//字节→数组
        {
            fixed (byte* pt = data)
            {
                for(int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    Items[i] = *item;
                }
            }
        }

        unsafe public NBagInfo GetBagInfo()//数组→字节
        {
            fixed (byte* pt = this.Info.Items)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
            }
            return this.Info;
        }

        public void AddItem(int itemId, int count)
        {
            ushort addCount = (ushort)count;
            for(int i = 0; i < Items.Length; i++)
            {
                if (this.Items[i].ItemId == itemId)
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[itemId].StackLimit - this.Items[i].Count);
                    if (canAdd > addCount)
                    {
                        this.Items[i].Count += addCount;
                        addCount = 0;
                        break;
                    }
                    else
                    {
                        this.Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }
            if (addCount > 0)
            {
                for(int i = 0; i < Items.Length; i++)
                {
                    if (this.Items[i].ItemId == 0)
                    {
                        this.Items[i].ItemId = (ushort)itemId;
                        if (DataManager.Instance.Items[itemId].StackLimit >= addCount)
                        {
                            this.Items[i].Count = addCount;
                            addCount = 0;
                            break;
                        }
                        else
                        {
                            this.Items[i].Count = (ushort)DataManager.Instance.Items[itemId].StackLimit;
                            addCount -= (ushort)DataManager.Instance.Items[itemId].StackLimit;
                        }
                    }
                }
                if (addCount > 0)
                {
                    MessageBox.Show("无法拿下更多东西", "提示", MessageBoxType.Error, "确定");
                }
            }
            //通知UI更新
            OnUpdateItems?.Invoke();
            SaveToServer();
        }
        public void RemoveItem(int itemId, int count)
        {
            int remaining = count;

            for (int i = 0; i < Items.Length && remaining > 0; i++)
            {
                if (Items[i].ItemId == itemId && Items[i].Count > 0)
                {
                    int remove = Math.Min(Items[i].Count, remaining);
                    Items[i].Count -= (ushort)remove;
                    remaining -= remove;

                    if (Items[i].Count == 0)
                    {
                        Items[i].ItemId = 0;
                    }
                }
            }

            if (remaining == 0)
            {
                OnUpdateItems?.Invoke();
                SaveToServer();
            }
        }

        private void SaveToServer()
        {
            if (this.Info == null)
                return;
            if (NetClient.Instance == null || !NetClient.Instance.Connected)
                return;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.bagSave = new BagSaveRequest();
            message.Request.bagSave.BagInfo = this.GetBagInfo();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 获取第一个空闲槽位索引，返回-1表示背包满
        /// </summary>
        public int GetFirstEmptySlot()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].ItemId == 0)
                    return i;
            }
            return -1;
        }
    }
}
