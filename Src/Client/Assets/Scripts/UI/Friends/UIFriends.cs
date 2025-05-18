using Common.Data;
using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriends : UIWindow
{
    public GameObject itemPrefab;
    public ListView listView;
    public Transform itemRoot;
    private UIFriendItem selectedItem;

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
        foreach (var info in FriendManager.Instance.allFriends)
        {
            if (info.Status == 1)
            {
                this.listView.AddItem<NFriendInfo, UIFriendItem>(info, itemPrefab);
                //GameObject go = Instantiate(itemPrefab, listView.transform);
                //UIFriendItem item = go.GetComponent<UIFriendItem>();
                //item.SetFriendInfo(info);
                //this.listView.AddItem(item);
            }
        }
        foreach (var info in FriendManager.Instance.allFriends)
        {
            if (info.Status != 1)
            {
                this.listView.AddItem<NFriendInfo, UIFriendItem>(info, itemPrefab);
                //GameObject go = Instantiate(itemPrefab, listView.transform);
                //UIFriendItem item = go.GetComponent<UIFriendItem>();
                //item.SetFriendInfo(info);
                //this.listView.AddItem(item);
            }
        }
    }

    private void OnFriendSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIFriendItem;
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("������Ҫ��ӵĺ������ƻ�ID", "��Ӻ���","���","ȡ��","���ݲ���Ϊ��").OnSubmit += OnFriendAddSubmit;
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

    public void OnClickTeamInvite()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("��ѡ��Ҫ����ĺ���");
            return;
        }
        if (selectedItem.Info.Status != 1)
        {
            MessageBox.Show("��ѡ�����ߵĺ���");
            return;
        }
        MessageBox.Show(string.Format("ȷ��ѡ�����롾{0}�����������", selectedItem.Info.friendInfo.Name), "����������", MessageBoxType.Confirm, "����", "ȡ��").OnYes = () =>
        {
            TeamService.Instance.SendTeamInviteResquest(this.selectedItem.Info.friendInfo.Id, this.selectedItem.Info.friendInfo.Name);
        };
    }

    public void OnClickFriendChat()
    {
        ChatManager.Instance.StartPrivateChat(selectedItem.Info.friendInfo.Id, selectedItem.Info.friendInfo.Name);
    }

    public void OnClickFriendRemove()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("��ѡ��Ҫɾ���ĺ���");
            return;
        }
        MessageBox.Show(string.Format("ȷ��Ҫɾ�����ѡ�{0}����", selectedItem.Info.friendInfo.Name), "ɾ������", MessageBoxType.Confirm, "ɾ��", "ȡ��").OnYes = () =>
        {
            FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.Info.Id, this.selectedItem.Info.friendInfo.Id);
        };
    }
}
