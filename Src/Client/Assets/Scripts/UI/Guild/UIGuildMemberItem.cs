using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{
    public NGuildMemberInfo Info;
    public TMP_Text name;
    public TMP_Text level;
    public TMP_Text type;
    public TMP_Text title;
    public TMP_Text joinTime;
    public TMP_Text status;

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

    internal void SetGuildInfo(NGuildMemberInfo info)
    {
        this.Info = info;
        if (this.name != null) this.name.text = this.Info.Info.Name;
        if (this.level != null) this.level.text = this.Info.Info.Level.ToString();
        if (this.type != null) this.type.text = this.Info.Info.Class.ToString();
        if (this.status != null) this.status.text = this.Info.Status == 1 ? "在线" : "离线";
        if (this.joinTime != null) this.joinTime.text = TimeUtils.GetTime(this.Info.joinTime).ToString();
        if (this.title != null) this.title.text = GuildManager.Instance.GetTitle(this.Info.Title);
    }
}
