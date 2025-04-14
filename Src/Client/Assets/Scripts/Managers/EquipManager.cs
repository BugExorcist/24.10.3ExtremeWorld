using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();

        public event OnEquipChangeHandler OnEquipChange;

        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];

        byte[] Data;//int list

        public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }

        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int)slot];
        }

        unsafe private void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.Data)
            {
                for(int i = 0; i < this.Equips.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId > 0)
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    else
                        Equips[i] = null;
                }
            }
        }

        unsafe public byte[] GetEquipData()
        {
            fixed (byte * pt = this.Data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemId = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                        *itemId = 0;
                    else
                        *itemId = Equips[i].Id;
                }
            }
            return this.Data;
        }

        public bool Contains(int equipId)//判断此ID的装备是否已经装备
        {
            for(int i = 0;i < this.Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipId)
                    return true;
            }
            return false;
        }


        //向服务器发送穿脱装备请求
        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }
        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }

        //实现服务器返回的穿脱装备回应
        public void OnEquipItem(Item equip)
        {
            if (this.Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
            {
                return;
            }
            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

            OnEquipChange?.Invoke();
        }

        public void OnUnEquipItem(EquipSlot slot)
        {
            if (this.Equips[(int)slot] != null)
            {
                this.Equips[(int)slot] = null;
                OnEquipChange?.Invoke();
            }
        }
    }
}
