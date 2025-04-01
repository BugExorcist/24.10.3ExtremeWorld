using Common.Data;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        { 
            NPCManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop);
        }

        private bool OnOpenShop(NpcDefine npc)
        {
            this.ShowShop(npc.Param);//这里输入的是np配置表中的商店ID
            return true;
        }

        private void ShowShop(int shopId)
        {
            ShopDefine shop;
            if(DataManager.Instance.Shops.TryGetValue(shopId, out shop))
            {
                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                if(uiShop != null)
                {
                    uiShop.SetShop(shop);
                }

            }
        }

        public bool BuyItem(int shopId, int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }
    }
}
