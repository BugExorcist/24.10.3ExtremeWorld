using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICharEquip : UIWindow
{

    public TMP_Text title;
    public TMP_Text money;
    public TMP_Text name;

    public GameObject itenPrefab;
    public GameObject equipPrefab;

    public Transform itenListRoot;

    public List<Transform> slots;
    void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChange += RefreshUI;
        User.Instance.OnUpdataGold += UpdataGold;
    }

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChange -= RefreshUI;
        User.Instance.OnUpdataGold -= UpdataGold;
    }

    private void RefreshUI()
    {
        this.name.text = string.Format("{0} LV{1}", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Level);
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipedList();
        InitEquipItems();
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    private void InitAllEquipItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type == ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class)
            {
                //已经装备就不显示
                if (EquipManager.Instance.Contains(kv.Key))
                    continue;
                GameObject go = Instantiate(itenPrefab, itenListRoot);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }
    private void ClearAllEquipList()
    {
        foreach (var item in itenListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }

    private void InitEquipItems()
    {
        for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i];
            if (item != null)
            {
                GameObject go = Instantiate(equipPrefab, slots[i]);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(i, item, this, true);
            }
        }
    }

    private void ClearEquipedList()
    {
        foreach (var item in slots)
        {
            var v = item.GetComponentInChildren<UIEquipItem>();
            if (v != null)
                Destroy(v.gameObject);
        }
    }

    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }

    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }

    private UIEquipItem currentItem = null;
    public void SelectItem(UIEquipItem item)
    {
        if (currentItem != null)
            currentItem.Selected = false;
        this.currentItem = item;
    }

    private void UpdataGold()
    {
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }
}
