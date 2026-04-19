using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    internal class ItemService : Singleton<ItemService>
    {
        public ItemService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(this.OnItemEquip);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemUseRequest>(this.OnItemUse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemSplitRequest>(this.OnItemSplit);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemDropRequest>(this.OnItemDrop);
        }


        public void Init()
        {

        }

        private void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnClickBuy: character:{0} Shop:{1} ShopItem:{2}", character.Id, request.shopId, request.shopItemId);
            var result = ShopManager.Instance.BuyItem(sender, request.shopId, request.shopItemId);
            sender.Session.Response.itemBuy = new ItemBuyResponse();
            sender.Session.Response.itemBuy.Result = result;
            sender.SendResponse();
        }

        private void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemEquip: character:{0} Slot:{1} Item:{2} Equip:{3}", character.Id, request.Slot, request.itemId, request.isEquip);
            var result = EquipManager.Instance.EquipItem(sender, request.Slot, request.itemId, request.isEquip);
            sender.Session.Response.itemEquip = new ItemEquipResponse();
            sender.Session.Response.itemEquip.Result = result;
            sender.SendResponse();
        }

        private void OnItemUse(NetConnection<NetSession> sender, ItemUseRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemUse: character:{0} SlotIndex:{1} Count:{2}", character.Id, request.slotIndex, request.Count);

            sender.Session.Response.itemUse = new ItemUseResponse();

            // 从背包字节中读取槽位对应的物品ID
            int itemId = GetItemIdFromSlot(character, request.slotIndex);
            if (itemId == 0)
            {
                sender.Session.Response.itemUse.Result = Result.Failed;
                sender.Session.Response.itemUse.Errormsg = "物品不存在";
                sender.SendResponse();
                return;
            }

            // 执行使用物品
            if (character.ItemManager.UseItem(itemId, request.Count))
            {
                sender.Session.Response.itemUse.Result = Result.Success;
                sender.Session.Response.itemUse.itemId = itemId;
                DBService.Instance.Save();
            }
            else
            {
                sender.Session.Response.itemUse.Result = Result.Failed;
                sender.Session.Response.itemUse.Errormsg = "无法使用该物品";
            }
            sender.SendResponse();
        }

        private void OnItemDrop(NetConnection<NetSession> sender, ItemDropRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemDrop: character:{0} SlotIndex:{1} Count:{2}", character.Id, request.slotIndex, request.Count);

            sender.Session.Response.itemDrop = new ItemDropResponse();

            if (request.Count <= 0)
            {
                sender.Session.Response.itemDrop.Result = Result.Failed;
                sender.Session.Response.itemDrop.Errormsg = "数量非法";
                sender.SendResponse();
                return;
            }

            int itemId = GetItemIdFromSlot(character, request.slotIndex);
            if (itemId == 0)
            {
                sender.Session.Response.itemDrop.Result = Result.Failed;
                sender.Session.Response.itemDrop.Errormsg = "物品不存在";
                sender.SendResponse();
                return;
            }

            int slotCount = GetItemCountFromSlot(character, request.slotIndex);
            if (slotCount < request.Count)
            {
                sender.Session.Response.itemDrop.Result = Result.Failed;
                sender.Session.Response.itemDrop.Errormsg = "数量不足";
                sender.SendResponse();
                return;
            }

            if (character.ItemManager.RemoveItem(itemId, request.Count))
            {
                sender.Session.Response.itemDrop.Result = Result.Success;
                sender.Session.Response.itemDrop.itemId = itemId;
                sender.Session.Response.itemDrop.Count = request.Count;
                DBService.Instance.Save();
            }
            else
            {
                sender.Session.Response.itemDrop.Result = Result.Failed;
                sender.Session.Response.itemDrop.Errormsg = "丢弃失败";
            }

            sender.SendResponse();
        }

        private void OnItemSplit(NetConnection<NetSession> sender, ItemSplitRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemSplit: character:{0} FromSlot:{1} ToSlot:{2} Count:{3}", character.Id, request.fromSlot, request.toSlot, request.Count);

            sender.Session.Response.itemSplit = new ItemSplitResponse();

            // 从背包字节中解析并拆分
            if (SplitBagItem(character, request.fromSlot, request.toSlot, request.Count))
            {
                sender.Session.Response.itemSplit.Result = Result.Success;
            }
            else
            {
                sender.Session.Response.itemSplit.Result = Result.Failed;
                sender.Session.Response.itemSplit.Errormsg = "拆分失败";
            }
            sender.SendResponse();
        }

        /// <summary>
        /// 从背包字节中读取指定槽位的物品ID
        /// BagItem结构: ushort ItemId(2字节) + ushort Count(2字节) = 4字节/槽位
        /// </summary>
        private int GetItemIdFromSlot(Character character, int slotIndex)
        {
            var bagItems = character.Data.Bag.Items;
            if (bagItems == null || bagItems.Length < (slotIndex + 1) * 4)
                return 0;

            int offset = slotIndex * 4;
            ushort itemId = BitConverter.ToUInt16(bagItems, offset);
            return itemId;
        }

        private int GetItemCountFromSlot(Character character, int slotIndex)
        {
            var bagItems = character.Data.Bag.Items;
            if (bagItems == null || bagItems.Length < (slotIndex + 1) * 4)
                return 0;

            int offset = slotIndex * 4;
            ushort count = BitConverter.ToUInt16(bagItems, offset + 2);
            return count;
        }

        /// <summary>
        /// 拆分背包物品
        /// </summary>
        private bool SplitBagItem(Character character, int fromSlot, int toSlot, int count)
        {
            var bagItems = character.Data.Bag.Items;
            int unlocked = character.Data.Bag.Unlocked;

            if (bagItems == null || fromSlot < 0 || fromSlot >= unlocked || toSlot < 0 || toSlot >= unlocked)
                return false;
            if (bagItems.Length < unlocked * 4)
                return false;

            // 读取源槽位
            int fromOffset = fromSlot * 4;
            ushort fromItemId = BitConverter.ToUInt16(bagItems, fromOffset);
            ushort fromCount = BitConverter.ToUInt16(bagItems, fromOffset + 2);

            // 源槽位没有物品或数量不足
            if (fromItemId == 0 || fromCount < count)
                return false;

            // 读取目标槽位
            int toOffset = toSlot * 4;
            ushort toItemId = BitConverter.ToUInt16(bagItems, toOffset);
            ushort toCount = BitConverter.ToUInt16(bagItems, toOffset + 2);

            // 目标槽位必须为空，或者是相同物品且未满
            if (toItemId != 0 && toItemId != fromItemId)
                return false;

            // 检查堆叠上限
            if (DataManager.Instance.Items.TryGetValue(fromItemId, out var define))
            {
                if (toItemId == fromItemId && toCount + count > define.StackLimit)
                    return false;
                if (count > define.StackLimit)
                    return false;
            }

            // 执行拆分
            ushort newFromCount = (ushort)(fromCount - count);
            ushort newToCount = (ushort)(toCount + count);

            // 写回源槽位
            byte[] fromCountBytes = BitConverter.GetBytes(newFromCount);
            bagItems[fromOffset + 2] = fromCountBytes[0];
            bagItems[fromOffset + 3] = fromCountBytes[1];

            // 如果源槽位清空，也清除ItemId
            if (newFromCount == 0)
            {
                bagItems[fromOffset] = 0;
                bagItems[fromOffset + 1] = 0;
            }

            // 写回目标槽位
            byte[] toItemIdBytes = BitConverter.GetBytes(fromItemId);
            byte[] toCountBytes = BitConverter.GetBytes(newToCount);
            bagItems[toOffset] = toItemIdBytes[0];
            bagItems[toOffset + 1] = toItemIdBytes[1];
            bagItems[toOffset + 2] = toCountBytes[0];
            bagItems[toOffset + 3] = toCountBytes[1];

            DBService.Instance.Save();
            return true;
        }
    }
}
