using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriends : UIWindow
{
    public GameObject itemPrefab;
    public ListView listView;
    public Transform itemRoot;
    public UIFriendItem selectedItem;

    private void Start()
    {
        FriendService.Instance.OnFriendUpdata = ReFreshUI;
        this.listView.onItemSelected += this.OnFriendSelected;
        ReFreshUI();
    }

    private void ReFreshUI()
    {
        ClaerFriendList();
        InitFriendItems();
    }

    private void ClaerFriendList()
    {
        this.listView.RemoveAll();
    }

    private void InitFriendItems()
    {
        //foreach (var kv in FriendManager.Instance.allQuests)
        //{
        //    if (showAvailableList)
        //    {   //��������Ϣ����ʾ�����Ѿ���ȡ������
        //        if (kv.Value.Info != null)
        //            continue;
        //    }
        //    else
        //    {   //��ʱ��������Ϣ����ʾ����δ��ȡ������
        //        if (kv.Value.Info == null)
        //            continue;
        //    }

        //    GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
        //    UIQuestItem item = go.GetComponent<UIQuestItem>();
        //    item.SetQuestInfo(kv.Value);
        //    this.listView.AddIten(item);
        //}
    }

    private void OnFriendSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIFriendItem;
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("������Ҫ��ӵĺ������ƻ�ID", "��Ӻ���").OnSubmit += OnFriendAddSubmit;
    }

    private bool OnFriendAddSubmit(string inputText, out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        if (!int.TryParse(inputText, out friendId))
            friendName = inputText;
        if (friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
        {
            tips = "��������Լ�Ϊ����Ŷ~";
            return false;
        }
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickFriendChar()
    {
        MessageBox.Show("��δʵ��");
    }

    public void OnClickFriendRemove()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("��ѡ��Ҫɾ���ĺ���");
            return;
        }
        MessageBox.Show(string.Format("ȷ��Ҫɾ������[{0}]��", selectedItem.Info.friendInfo.Name), "ɾ������", MessageBoxType.Confirm, "ɾ��", "ȡ��");
        FriendService.Instance.
    }
}
