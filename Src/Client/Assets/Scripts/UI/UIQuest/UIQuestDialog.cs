using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestDialog : UIWindow
{
    public UIQuestInfo questInfo;
    public GameObject openButton;
    public GameObject submitButton;
    public Quest quest;
    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        UpdataQuestInfo(this.quest);
        if (this.quest.Info == null)//无网络信息 任务还没接受
        {
            openButton.SetActive(true);
            submitButton.SetActive(false);
        }
        else
        {
            openButton.SetActive(false );
            submitButton.SetActive(false);
            if (this.quest.Info.Status == SkillBridge.Message.QuestStatus.Completed)
                submitButton.SetActive(true);
        }
    }

    private void UpdataQuestInfo(Quest quest)
    {
        if (quest != null)
        {
            if (this.questInfo != null)
            {
                this.questInfo.SetQuestInfo(this.quest);
            }
        }
    }
}
