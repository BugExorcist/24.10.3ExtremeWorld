using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class UINpcQuestStatus : MonoBehaviour
{
    public Image[] statusImages;

    private NpcQuestStatus questStatus;
    public void SetQuestStatus(NpcQuestStatus status)
    {
        this.questStatus = status;
        for(int i = 0; i < statusImages.Length; i++)
        {
            if (this.statusImages[i] != null)
                this.statusImages[i].gameObject.SetActive(i == (int)status);
        }
    }
}

