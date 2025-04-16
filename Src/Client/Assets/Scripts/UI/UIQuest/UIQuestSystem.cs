using Common.Data;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIQuestSystem : UIWindow
{
    public TMP_Text title;
    public GameObject itemPrefab;

    public UITabView Tabs;
    public ListView listMain;
    public ListView listBranch;

    public UIQuestInfo qurstInfo;

    private bool showAvailableList = false;//�Ƿ���ʾ�ɽ�����

    private void Start()
    {
        this.listMain.onItemSelected += this.OnQuestSelected;
        this.listBranch.onItemSelected += this.OnQuestSelected;
        this.Tabs.onTabSelect += OnSelectTab;
        RefreshUI();
    }

    private void OnDestroy()
    {
        this.listMain.onItemSelected -= this.OnQuestSelected;
        this.listBranch.onItemSelected -= this.OnQuestSelected;
        this.Tabs.onTabSelect -= OnSelectTab;
    }
    private void OnQuestSelected(ListView.ListViewItem item)
    {
        UIQuestItem questItem = item as UIQuestItem;
        this.qurstInfo.SetQuestInfo(questItem.quest);
    }

    void OnSelectTab(int idx)
    {
        showAvailableList = idx == 1;
        RefreshUI();
    }

    private void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }

    private void InitAllQuestItems()
    {
        foreach (var kv in QuestManager.Instance.allQuests)
        {
            if (showAvailableList)
            {   //��������Ϣ����ʾ�����Ѿ���ȡ������
                if (kv.Value.Info != null)
                    continue;
            }
            else
            {   //��ʱ��������Ϣ����ʾ����δ��ȡ������
                if (kv.Value.Info == null)
                    continue;
            }

            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
            UIQuestItem item = go.GetComponent<UIQuestItem>();
            item.SetQuestInfo(kv.Value);
            if (kv.Value.Define.Type == QuestType.Main)
                this.listMain.AddIten(item);
            else
                this.listBranch.AddIten(item);
        }
    }
    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }
}
