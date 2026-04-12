using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ItemManager
    {
        Character Owner;

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public ItemManager(Character owner)
        {
            this.Owner = owner;
            
            foreach (var item in owner.Data.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));
            }
        }

        public bool UseItem(int itemId, int count = 1)
        {
            Log.InfoFormat("[{0}]UseItem[{1}:{2}]", this.Owner.Data.ID, itemId, count);
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                if (item.Count < count)
                {
                    return false;
                }

                var define = DataManager.Instance.Items[itemId];
                if (define == null || !define.CanUse)
                    return false;

                // 执行物品效果
                ExecuteItemEffect(define, count);

                item.Remove(count);
                this.Owner.StatusManager.AddItemChange(itemId, count, StatusAction.Delete);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 执行物品效果
        /// </summary>
        private void ExecuteItemEffect(ItemDefine define, int count)
        {
            int totalValue = define.Param * count;

            switch (define.Function)
            {
                case ItemFunction.RecoverHP:
                    // TODO:血量修改
                    Log.InfoFormat("[{0}]RecoverHP:{1}  目前还未实现！", this.Owner.Data.ID, define.Param);
                    
                    break;
                case ItemFunction.RecoverMP:
                    // TODO: 法力修改
                    Log.InfoFormat("[{0}]RecoverMP:{1}  目前还未实现！", this.Owner.Data.ID, define.Param);
                    
                    break;
                case ItemFunction.AddBuff:
                    // TODO: BuffManager 接入
                    Log.InfoFormat("[{0}]AddBuff:{1}  目前还未实现！", this.Owner.Data.ID, define.Param);
                    break;
                case ItemFunction.AddExp:
                    this.Owner.AddExp(totalValue);
                    break;
                case ItemFunction.AddMoney:
                    this.Owner.Gold += totalValue;
                    break;
                case ItemFunction.AddItem:
                    // TODO: 随机生成物品
                    Log.InfoFormat("[{0}]AddItem:{1}  目前还未实现！", this.Owner.Data.ID, define.Param);
                    break;
                case ItemFunction.AddSkillPoint:
                    // TODO: 技能点系统
                    Log.InfoFormat("[{0}]AddSkillPoint:{1}  目前还未实现！", this.Owner.Data.ID, define.Param);
                    break;
            }
        }

        public bool HasItem(int itemId)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                return item.Count > 0;
            }
            return false;
        }

        public Item GetItem(int itemId)
        {
            Item item = null;
            this.Items.TryGetValue(itemId, out item);
            Log.InfoFormat("[{0}]GetItem[{1}:{2}]",this.Owner.Data.ID, itemId, item);
            return item;
        }

        public bool AddItem(int itemId, int count)
        {
            Item item = null;
            if(this.Items.TryGetValue(itemId, out item))
            {
                item.Add(count);
            }
            else
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);
                item = new Item(dbItem);
                this.Items.Add(itemId, item);
            }
            this.Owner.StatusManager.AddItemChange(itemId, count, StatusAction.Add);
            Log.InfoFormat("[{0}]AddItem:[{1}]addCount:[{2}]", this.Owner.Data.ID, item, count);
            //DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int itemId, int count)
        {
            if (!this.Items.ContainsKey(itemId))
            {
                return false;
            }
            Item item = this.Items[itemId];
            if(item.Count < count)
            {
                return false;
            }
            item.Remove(count);
            this.Owner.StatusManager.AddItemChange(itemId, count, StatusAction.Delete);
            Log.InfoFormat("[{0}]RemoveItem[{1}]RemoveCount:[{2}]", this.Owner.Data.ID, item, count);
            //DBService.Instance.Save();
            return true;
        }

        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach (var item in this.Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    }
}
