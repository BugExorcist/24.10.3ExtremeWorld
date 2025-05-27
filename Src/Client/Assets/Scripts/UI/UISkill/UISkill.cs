using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISkill : UIWindow
{
    public TMP_Text descript;
    public GameObject itemPrefab;
    public ListView listView;
    private UISkillItem selectedItem;

    private void Start()
    {
        ReFreshUI();
        this.listView.onItemSelected += this.OnItemSelected;
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UISkillItem;
        this.descript.text = this.selectedItem.item.Description;
    }

    private void ReFreshUI()
    {
        ClearItems();
        InitItems();
    }

    private void InitItems()
    {
        var Skills = DataManager.Instance.Skills[(int)User.Instance.CurrentCharacterInfo.Class];
        foreach (var kv in Skills)
        {
            if (kv.Value.Type == Common.Battle.SkillType.Skill)
            {
                GameObject go = Instantiate(itemPrefab, this.listView.transform);
                UISkillItem ui = go.GetComponent<UISkillItem>();
                ui.SetItemInfo(kv.Value, this, false);
                this.listView.AddItem(ui);
            }
        }
    }
    private void ClearItems()
    {
        this.listView.RemoveAll();
    }
}
