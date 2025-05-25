using Common.Battle;
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharEquip : UIWindow
{

    public TMP_Text title;
    public TMP_Text money;
    public TMP_Text name;

    public GameObject itenPrefab;
    public GameObject equipPrefab;

    public Transform itenListRoot;

    public List<Transform> slots;

    public TMP_Text hp;
    public Slider hpBar;
    public TMP_Text mp;
    public Slider mpBar;
    public TMP_Text[] attrs;


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
        this.name.text = string.Format("{0} LV{1}", User.Instance.CurrentCharacterInfo.Name, User.Instance.CurrentCharacterInfo.Level);
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipedList();
        InitEquipItems();
        this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();

        InitAttributes();
    }

    private void InitAllEquipItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type == ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacterInfo.Class)
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
        this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
    }

    public void InitAttributes()
    {
        var attribute = User.Instance.CurrentCharacter.Attributes;
        this.hp.text = string.Format("{0}/{1}", attribute.HP, attribute.MaxHP);
        this.mp.text  = string.Format("{0}/{1}", attribute.MP, attribute.MaxMP);
        this.hpBar.maxValue = attribute.MaxHP;
        this.hpBar.value = attribute.HP;
        this.mpBar.maxValue = attribute.MaxMP;
        this.mpBar.value = attribute.MP;

        for(int i = (int)AttributeType.STR; i < (int)AttributeType.MAX; i++)
        {
            if (i == (int)AttributeType.CRI)
            {
                this.attrs[i - 2].text = string.Format("{0:f2}%", attribute.Final.Data[i] * 100);
            }
            else
            {
                this.attrs[i - 2].text = ((int)attribute.Final.Data[i]).ToString();
            }
        }
    }
}
