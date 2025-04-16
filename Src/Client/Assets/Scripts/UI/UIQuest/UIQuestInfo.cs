using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Models;
using SkillBridge.Message;
using System.ComponentModel;

public class UIQuestInfo : MonoBehaviour 
{
    public TMP_Text title;
    public TMP_Text[] targets;
    public TMP_Text description;

    public UIIconItem rewardItems;

    public TMP_Text rewardMoney;
    public TMP_Text rewardExp;
    internal void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
        if (quest.Info == null)
            this.description.text = quest.Define.Dialog;
        else
        {
            if (quest.Info.Status == QuestStatus.Completed)
                this.description.text = quest.Define.DialogFinish;
        }
        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();

        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

    public void OnClickAbandon()
    {

    }
}

