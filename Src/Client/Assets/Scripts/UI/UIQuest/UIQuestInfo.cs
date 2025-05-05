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
using Managers;

public class UIQuestInfo : MonoBehaviour 
{
    public TMP_Text title;
    public TMP_Text[] targets;
    public TMP_Text description;

    public Image[] rewardItems;
    public GameObject rewardPrefab;

    public TMP_Text rewardMoney;
    public TMP_Text rewardExp;
    internal void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type.GetDescription(), quest.Define.Name);
        if (quest.Info == null)
        {   //未接取
            this.description.text = quest.Define.Dialog;
        }
        else
        {   //已接取
            if (quest.Info.Status == QuestStatus.Finished)
                this.description.text = quest.Define.DialogFinish;
            else
                this.description.text = quest.Define.Dialog;
        }

        foreach (var item in rewardItems)
        {
            foreach (Transform child in item.transform)
                Destroy(child.gameObject);
            item.gameObject.SetActive(false);
        }
            
        if (quest.Define.RewardItem1 > 0)
        {
            rewardItems[0].gameObject.SetActive(true);
            GameObject go = Instantiate(rewardPrefab, rewardItems[0].transform);
            UIIconItem item = go.GetComponent<UIIconItem>();
            item.SetMainIcon(DataManager.Instance.Items[quest.Define.RewardItem1].Icon , quest.Define.RewardItem1Count.ToString());
        }
        if (quest.Define.RewardItem2 > 0)
        {
            rewardItems[1].gameObject.SetActive(true);
            GameObject go = Instantiate(rewardPrefab, rewardItems[0].transform);
            UIIconItem item = go.GetComponent<UIIconItem>();
            item.SetMainIcon(DataManager.Instance.Items[quest.Define.RewardItem2].Icon, quest.Define.RewardItem2Count.ToString());
        }
        if (quest.Define.RewardItem3 > 0)
        {
            rewardItems[2].gameObject.SetActive(true);
            GameObject go = Instantiate(rewardPrefab, rewardItems[0].transform);
            UIIconItem item = go.GetComponent<UIIconItem>();
            item.SetMainIcon(DataManager.Instance.Items[quest.Define.RewardItem3].Icon, quest.Define.RewardItem3Count.ToString());
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

