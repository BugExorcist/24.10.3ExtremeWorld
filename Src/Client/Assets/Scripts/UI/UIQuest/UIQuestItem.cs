using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem
{
    public TMP_Text title;
    public Image background;
    private Sprite normalBg;
    public Sprite selectedBg;

    private void Start()
    {
        this.normalBg = background.sprite;
    }

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public Quest quest;

    public void SetQuestInfo(Quest item)
    {
        this.quest = item;
        if (this.title != null)
            this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
    }
}

