using Common;
using Common.Data;
using Common.Battle;  // 确保引入Attributes所在的命名空间
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            Character character = sender.Session.Character;
            if(!character.ItemManager.Items.ContainsKey(itemId))
                return Result.Failed;
            
            character.Data.Equips = UpdataEquip(character.Data.Equips, slot, itemId, isEquip);
            
            // 更新角色属性
            UpdateCharacterAttributes(character);
            
            DBService.Instance.Save();
            return Result.Success;
        }

        private void UpdateCharacterAttributes(Character character)
        {
            // 获取当前装备的EquipDefine列表
            List<EquipDefine> equipDefines = new List<EquipDefine>();
            byte[] equipData = character.Data.Equips;
            
            // 解析装备数据并获取对应的EquipDefine
            unsafe
            {
                fixed (byte* pt = equipData)
                {
                    for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                    {
                        int itemId = *(int*)(pt + i * sizeof(int));
                        if (itemId > 0 && DataManager.Instance.Items.ContainsKey(itemId))
                        {
                            var item = DataManager.Instance.Items[itemId];
                            if (item.Type == ItemType.Equip && DataManager.Instance.Equips.ContainsKey(itemId))
                            {
                                equipDefines.Add(DataManager.Instance.Equips[itemId]);
                            }
                        }
                    }
                }
            }
            
            // 更新角色属性
            character.Attributes.UpdateEquip(equipDefines);
        }

        unsafe private byte[] UpdataEquip(byte[] equipData, int slot, int itemId, bool isEquip)
        {
            byte[] EquipData = new byte[28];
            fixed(byte* pt = equipData)
            {
                int* slotid = (int*)(pt + slot * sizeof(int));
                if (isEquip)
                    *slotid = itemId;//穿
                else
                    *slotid = 0;//脱
            }

            Array.Copy(equipData, EquipData, 28);
            return EquipData;
        }
    }
}
