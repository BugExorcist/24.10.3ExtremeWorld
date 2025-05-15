using Managers;
using Services;
using SkillBridge.Message;
using System;
using UnityEngine;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;
    public ListView listView;
    public UIGuildInfo uiInfo;
    private UIGuildMemberItem selectedItem;

    public GameObject panelAdmin;//管理员
    public GameObject panelLeader;//会长

    private void Start()
    {
        this.listView.onItemSelected += this.OnItemSelected;
        GuildService.Instance.OnGuildUpdate += UpdateUI;
        UpdateUI();
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }

    void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;
        ClearList();
        InitItems();

        this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);
        this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.President);
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildMemberItem;
    }

    private void InitItems()
    {
        foreach (var info in GuildManager.Instance.guildInfo.Members)
        {
            this.listView.AddItem<NGuildMemberInfo, UIGuildMemberItem>(info, itemPrefab);
            //GameObject go = Instantiate(itemPrefab, listView.transform);
            //UIGuildMemberItem item = go.GetComponent<UIGuildMemberItem>();
            //item.SetGuildInfo(info);
            //this.listView.AddItem(item);
        }
    }

    private void ClearList()
    {
        this.listView.RemoveAll();
    }

    public void OnClickApplyList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }
    public void OnClickLeave()
    {
        if (GuildManager.Instance.myMemberInfo.Title == GuildTitle.President)
        {
            if(GuildManager.Instance.guildInfo.Members.Count > 1)
            {
                MessageBox.Show("会长无法退出公会，请先转让会长！", "警告", MessageBoxType.Information);
                return;
            }
        }
        MessageBox.Show("确定要退出公会吗？", "提示", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildLeaveRequest();
        };
    }

    public void OnClickChat()
    {
        MessageBox.Show("敬请期待");
    }
    public void OnClickKickOut()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要踢出的玩家！");
            return;
        }
        MessageBox.Show(string.Format("确定要踢出玩家【{0}】吗？", selectedItem.Info.Info.Name), "踢出玩家", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.selectedItem.Info.Info.Id);
        };
    }
    public void OnClickPromote()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要晋升的玩家！");
            return;
        }
        if (selectedItem.Info.Title != GuildTitle.None)
        {
            MessageBox.Show("请选择普通成员！");
            return;
        }
        MessageBox.Show(string.Format("确定要晋升玩家【{0}】为副会长吗？", selectedItem.Info.Info.Name), "晋升", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id);
        };
    }
    public void OnClickDepose()
    {
        if  (selectedItem == null)
        {
            MessageBox.Show("请选择要罢免的玩家！");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.President)
        {
            MessageBox.Show("无法罢免会长！");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.None)
        {
            MessageBox.Show("对方没有职务！");
            return;
        }
        MessageBox.Show(string.Format("确定要罢免玩家【{0}】吗？", selectedItem.Info.Info.Name), "罢免", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depose, this.selectedItem.Info.Info.Id);
        };
    }
    public void OnClickTransfer()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要转让的成员！");
            return;
        }
        MessageBox.Show(string.Format("确定要转让会长给玩家【{0}】吗？", selectedItem.Info.Info.Name), "转让会长", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id);
        };
    }
    public void OnClickSetNotice()
    {
        InputBox.Show("请输入公会宣言", "设置公会宣言", "确定", "取消", "宣言不能为空").OnSubmit += SendNotice;
    }

    private bool SendNotice(string inputText, out string tips)
    {
        tips = "";
        if (inputText.Length < 3 || inputText.Length > 50)
        {
            tips = "公会宣言长度为3-50个字符";
            return false;
        }
        GuildService.Instance.SendGuildSetNotice(inputText);
        return true;
    }
}
