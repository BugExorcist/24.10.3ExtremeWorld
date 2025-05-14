using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem
{
    public NGuildInfo Info;
    public TMP_Text id;
    public TMP_Text name;
    public TMP_Text memberCount;
    public TMP_Text leader;

    public Image Bg;
    private Sprite normalBg;
    public Sprite selectedBg;

    private void Start()
    {
        normalBg = Bg.GetComponent<Sprite>();
    }

    public override void onSelected(bool selected)
    {
        this.Bg.overrideSprite = selected ? selectedBg : normalBg;
    }

    internal void SetGuildInfo(NGuildInfo info)
    {
        this.Info = info;
        if (this.name != null) this.name.text = this.Info.GuildName;
        if (this.id != null) this.id.text = this.Info.Id.ToString();
        if (this.memberCount != null) this.memberCount.text = this.Info.memberCount.ToString();
        if (this.leader != null) this.leader.text = this.Info.leaderName;
    }
}

