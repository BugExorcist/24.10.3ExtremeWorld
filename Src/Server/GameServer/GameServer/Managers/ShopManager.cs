using Common;
using Common.Data;
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
    class ShopManager : Singleton<ShopManager>
    {
        public Result BuyItem(NetConnection<NetSession> sender, int shopId, int shopItemId)
        {
            if (!DateManager.Instance.Shops.ContainsKey(shopId))
            {
                return Result.Failed;
            }
            ShopItemDefine shopItem;
            if (DateManager.Instance.ShopItems[shopId].TryGetValue(shopItemId, out shopItem))
            {
                Log.InfoFormat("BuyItem:  Character:{0} Item:{1}  Count:{2}  Price :{3}", sender.Session.Character.Id, shopItem.ItemID, shopItem.Count, shopItem.Prise);
                if (sender.Session.Character.Gold >= shopItem.Prise)
                {
                    sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);
                    sender.Session.Character.Gold -= shopItem.Prise;
                    DBService.Instance.Save();
                    return Result.Success;
                }
            }
            return Result.Failed;
        }
    }
}
