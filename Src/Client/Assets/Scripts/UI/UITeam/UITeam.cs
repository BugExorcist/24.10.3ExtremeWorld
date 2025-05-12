using Models;
using Services;
using System;
using TMPro;
using UnityEngine;

public class UITeam : MonoBehaviour
{
    public TMP_Text teamTitle;
    public UITeamItem[] Members;
    public ListView list;

    private void Start()
    {
        if (User.Instance.TeamInfo == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        foreach (var item in Members)
        {
            this.list.AddIten(item);
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
    }
    public void OnClickLeave()
    {
        MessageBox.Show("确定离开队伍吗？", "离开队伍", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamLeaveRequest();
        };
    }
}
