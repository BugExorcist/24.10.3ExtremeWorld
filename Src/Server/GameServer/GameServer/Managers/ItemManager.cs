using Common;
using Common.Data;
using GameServer.Battle;
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
                if (!ExecuteItemEffect(define, count))
                    return false;

                item.Remove(count);
                this.Owner.StatusManager.AddItemChange(itemId, count, StatusAction.Delete);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 执行物品效果
        /// </summary>
        private bool ExecuteItemEffect(ItemDefine define, int count)
        {
            int totalValue = define.Param * count;

            switch (define.Function)
            {
                case ItemFunction.RecoverHP:
                    this.Owner.Attributes.HP += totalValue;
                    return true;
                case ItemFunction.RecoverMP:
                    this.Owner.Attributes.MP += totalValue;
                    return true;
                case ItemFunction.AddBuff:
                    if (!DataManager.Instance.Buffs.TryGetValue(define.Param, out var buffDefine))
                    {
                        Log.WarningFormat("[{0}]AddBuff failed: BuffDefine[{1}] not found", this.Owner.Data.ID, define.Param);
                        return false;
                    }
                    if (this.Owner.Map == null || this.Owner.Map.Battle == null)
                    {
                        Log.WarningFormat("[{0}]AddBuff failed: map or battle not ready", this.Owner.Data.ID);
                        return false;
                    }
                    var context = new BattleContext(this.Owner.Map.Battle)
                    {
                        Caster = this.Owner,
                        Target = this.Owner
                    };
                    this.Owner.AddBuff(context, buffDefine);
                    return true;
                case ItemFunction.AddExp:
                    this.Owner.AddExp(totalValue);
                    return true;
                case ItemFunction.AddMoney:
                    this.Owner.Gold += totalValue;
                    return true;
                case ItemFunction.AddItem:
                    Log.WarningFormat("[{0}]AddItem:{1} not implemented", this.Owner.Data.ID, define.Param);
                    return false;
                case ItemFunction.AddSkillPoint:
                    Log.WarningFormat("[{0}]AddSkillPoint:{1} not implemented", this.Owner.Data.ID, define.Param);
                    return false;
            }

            return false;
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
