using Models;
using Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITeam : MonoBehaviour
{
    public TMP_Text teamTitle;
    public UITeamItem[] Members;
    public ListView list;
    public GameObject LeaderPanel;
    private UITeamItem selectedItem;

    private void Start()
    {
        this.list.onItemSelected += this.OnMemberSelected;
        if (User.Instance.TeamInfo == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        foreach (var item in Members)
        {
            this.list.AddItem(item);
        }
    }

    void OnEnable()
    {
        UpdateTeamUI();
    }

    public void ShowTeam(bool show)
    {
        this.gameObject.SetActive(show);
        if (show)
            UpdateTeamUI();
    }
    private void UpdateTeamUI()
    {
        if (User.Instance.TeamInfo == null) return;
        this.teamTitle.text = String.Format("我的队伍 ({0}/5)", User.Instance.TeamInfo.Members.Count);

        for (int i = 0; i < 5; i++)
        {
            if (i < User.Instance.TeamInfo.Members.Count)
            {
                this.Members[i].SetMemnerInfo(i, User.Instance.TeamInfo.Members[i], User.Instance.TeamInfo.Members[i].Id == User.Instance.TeamInfo.Leader);
                this.Members[i].gameObject.SetActive(true);
            }
            else
            {
                this.Members[i].gameObject.SetActive(false);
            }
        }
        this.LeaderPanel.SetActive(User.Instance.TeamInfo.Leader == User.Instance.CurrentCharacterInfo.Id);
        

    }

    private void OnMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UITeamItem;
    }

    public void OnClickLeave()
    {
        MessageBox.Show("确定离开队伍吗？", "离开队伍", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamLeaveRequest();
        };
    }

    public void OnClickKickOut()
    {
        if (this.selectedItem.chaInfo.Id == User.Instance.CurrentCharacterInfo.Id)
            OnClickLeave();
        else
        {
            MessageBox.Show(string.Format("确定要踢出【{0}】吗？", this.selectedItem.chaInfo.Name), "踢出队伍", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
            {
                TeamService.Instance.SendTeamLeaveRequest(this.selectedItem.chaInfo.Id);
            };
        }
    }

    public void OnClickSetLeader()
    {
        if (this.selectedItem.chaInfo.Id == User.Instance.CurrentCharacterInfo.Id)
        {
            MessageBox.Show("你已经是队长了", "提示", MessageBoxType.Information);
            return;
        }
        MessageBox.Show(string.Format("确定要设置【{0}】为队长吗？", this.selectedItem.chaInfo.Name), "更换队长", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            TeamService.Instance.SendSetLeader(this.selectedItem.chaInfo.Id);
        };
    }
}
