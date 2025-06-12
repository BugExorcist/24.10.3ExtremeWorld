using Entities;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UIWindow;

public class UIMain : MonoSingleton<UIMain>
{
    public TMP_Text avaterName;
    public TMP_Text avaterLevel;
    public TMP_Text id;

    public UITeam TeamWindow;

    public UICreatureInfo targetUI;

    public UISkillSlots skillSlots;
    protected override void OnStart()
    {
        UpdateUIAvatar();
        this.targetUI.gameObject.SetActive(false);
        BattleManager.Instance.OnTargetChanged += OnTargetChanged;
        User.Instance.OnCharacterInit += this.skillSlots.UpdateSkills;
        this.skillSlots.UpdateSkills();
    }

    void UpdateUIAvatar()
    {
        this.avaterName.text = User.Instance.CurrentCharacterInfo.Name;
        this.avaterLevel.text = User.Instance.CurrentCharacterInfo.Level.ToString();
        this.id.text = string.Format("ID:{0}", User.Instance.CurrentCharacterInfo.Id);
    }


    public void BackToCharacterSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }

    public void OnClickbag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickEquip()
    {
       UIManager.Instance.Show<UICharEquip>();
    }
    
    public void OnClickQuest()
    {
       UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }

    public void OnCilckGuild()
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()
    {
        UIManager.Instance.Show<UIRide>();
    }

    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }

    public void OnClickSkill()
    {
        UIManager.Instance.Show<UISkill>();
    }

    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }

    private void OnTargetChanged(Creature target)
    {
        if (target != null)
        {
            if (!this.targetUI.isActiveAndEnabled) this.targetUI.gameObject.SetActive(true);
            targetUI.Target = target;
        }
        else
        {
            this.targetUI.gameObject.SetActive(false);
        }
    }
}
