using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIRide : UIWindow
{
    public TMP_Text descript;
    public GameObject itemPrefab;
    public ListView listView;
    private UIRideItem selectedItem;

    private void Start()
    {
        ReFreshUI();
        this.listView.onItemSelected += this.OnItemSelected;
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIRideItem;
        this.descript.text = this.selectedItem.item.Define.Description;
    }

    private void ReFreshUI()
    {
        ClearItems();
        InitItems();
    }

    private void InitItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type == ItemType.Ride && (kv.Value.Define.LimitClass == User.Instance.CurrentCharacterInfo.Class || kv.Value.Define.LimitClass == CharacterClass.None))
            {
                GameObject go = Instantiate(itemPrefab, this.listView.transform);
                UIRideItem ui = go.GetComponent<UIRideItem>();
                ui.SetItemInfo(kv.Value, this, false);
                this.listView.AddItem(ui);
            }
        }
    }
    private void ClearItems()
    {
        this.listView.RemoveAll();
    }

    public void DoRide()
    {
        if(this.selectedItem == null)
        {
            MessageBox.Show("ÇëÑ¡ÔñÒªÆïµÄ×øÆï£¡");
            return;
        }
        User.Instance.Ride(this.selectedItem.item.Id);
    }
}
