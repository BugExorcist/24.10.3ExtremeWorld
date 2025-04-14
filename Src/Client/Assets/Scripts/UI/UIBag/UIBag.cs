using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    public TMP_Text title;

    public TMP_Text money;

    public Transform[] pages;

    public GameObject bagItems;//UIBagItem的预制体

    List<Image> slots;//每个格子的Image

    private void Start()
    {
        //监听UI更新
        User.Instance.OnUpdataGold += UpdataGold;
        BagManager.Instance.OnUpdateItems += UpdateItems;

        if (slots == null)
        {
            slots = new List<Image>();
            for (int page = 0; page < pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        StartCoroutine(InitBags());
    }

    private void OnDestroy()
    {
        User.Instance.OnUpdataGold -= UpdataGold;
        BagManager.Instance.OnUpdateItems -= UpdateItems;
    }

        IEnumerator InitBags()
    {
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId > 0)
            {
                GameObject go = Instantiate(bagItems, slots[i].transform);
                UIIconItem ui = go.GetComponent<UIIconItem>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString());
            }
        }
        for (int i = BagManager.Instance.Unlocked; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }
        yield return null;
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
    }

    private void UpdataGold()
    {
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    private void UpdateItems()
    {
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            UIIconItem ui = slots[i].transform.GetComponentInChildren<UIIconItem>();
            if (item.ItemId <= 0 && ui != null)
            {
                foreach (Transform chile in slots[i].transform)
                {
                    Destroy(chile);
                }
            }
            else if (item.ItemId > 0)
            {
                if (ui != null)
                {
                    var def = ItemManager.Instance.Items[item.ItemId].Define;
                    ui.SetMainIcon(def.Icon, item.Count.ToString());
                }
                else
                {
                    GameObject go = Instantiate(bagItems, slots[i].transform);
                    ui = go.GetComponent<UIIconItem>();
                    var def = ItemManager.Instance.Items[item.ItemId].Define;
                    ui.SetMainIcon(def.Icon, item.Count.ToString());
                }
            }
        }
    }
}
