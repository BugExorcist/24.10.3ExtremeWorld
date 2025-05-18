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
        InputBox.Show("请输入要添加的好友名称或ID", "添加好友","添加","取消","数据不能为空").OnSubmit += OnFriendAddSubmit;
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
            tips = "不能添加自己为好友哦~";
            return false;
        }
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickTeamInvite()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要邀请的好友");
            return;
        }
        if (selectedItem.Info.Status != 1)
        {
            MessageBox.Show("请选择在线的好友");
            return;
        }
        MessageBox.Show(string.Format("确定选择邀请【{0}】加入队伍吗？", selectedItem.Info.friendInfo.Name), "邀请好友组队", MessageBoxType.Confirm, "邀请", "取消").OnYes = () =>
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
            MessageBox.Show("请选择要删除的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要删除好友【{0}】吗？", selectedItem.Info.friendInfo.Name), "删除好友", MessageBoxType.Confirm, "删除", "取消").OnYes = () =>
        {
            FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.Info.Id, this.selectedItem.Info.friendInfo.Id);
        };
    }
}
