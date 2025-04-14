using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIShop : UIWindow
{
    public TMP_Text title;

    public TMP_Text money;

    public GameObject shopItem;//UIShopItem��Ԥ����

    ShopDefine shop;

    public Transform itemRoot;


    private void Start()
    {
        //����UI����
        User.Instance.OnUpdataGold += UpdataGold;

        StartCoroutine(InitItems());
    }

    private void OnDestroy()
    {
        User.Instance.OnUpdataGold -= UpdataGold;
    }
    IEnumerator InitItems()
    {
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)
            {
                GameObject go = Instantiate(shopItem, itemRoot);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.setShopItem(kv.Key, kv.Value, this);
            }

        }
        yield return null;
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    private UIShopItem selectedItem;
    internal void SelectShopItem(UIShopItem uiShopItem)
    {
        if (selectedItem != null)
        {   //��ǰ��ѡ�е�UI
            selectedItem.Selected = false;
        }
        this.selectedItem = uiShopItem;
    }

    public void OnClickBuy()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("��ѡ��Ҫ����ĵ���", "������ʾ");
            return;
        }
        if (!ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.ShopItemID))
        {
            MessageBox.Show("����ʧ��");
        }
    }

    private void UpdataGold()
    {
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }
}
